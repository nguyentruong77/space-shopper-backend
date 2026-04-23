using AutoMapper;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Orders;
using SpaceShopper.Application.Interfaces.IRepositories.Promotions;
using SpaceShopper.Application.Interfaces.IRepositories.Shipping;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Orders;
using SpaceShopper.Application.Requests.Orders;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.Entities.Orders;
using SpaceShopper.Domain.Entities.Promotions;
using SpaceShopper.Domain.Entities.Shipping;
using SpaceShopper.Domain.Entities.Users;
using SpaceShopper.Domain.Enums;

namespace SpaceShopper.Application.Services.Orders
{
    public sealed class OrderService(
        IUserRepository userRepository,
        IProductRepository productRepository,
        IMethodShippingRepository methodShippingRepository,
        IPromotionRepository promotionRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IOrderService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMethodShippingRepository _methodShippingRepository = methodShippingRepository;
        private readonly IPromotionRepository _promotionRepository = promotionRepository;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PreCheckoutDto> PreCheckoutAsync(Guid userId, PreCheckoutRequest request, CancellationToken cancellationToken = default)
        {
            var context = await BuildCheckoutContextAsync(
                userId,
                request.ListItems,
                request.OrderPromotionCode,
                request.ShippingPromotionCode,
                shippingMethod: null,
                requireShipping: false,
                shippingAddressId: null,
                asNoTracking: true,
                cancellationToken);

            return ToPreCheckoutDto(context);
        }

        public async Task<CheckoutResultDto> CheckoutAsync(Guid userId, CheckoutRequest request, CancellationToken cancellationToken = default)
        {
            var shippingAddressId = request.ShippingAddressId
                ?? throw new ValidationException(ErrorCodes.Checkout.ShippingAddressRequired, ErrorMessages.Checkout.ShippingAddressRequired);

            var context = await BuildCheckoutContextAsync(
                userId,
                request.ListItems,
                request.OrderPromotionCode,
                request.ShippingPromotionCode,
                request.ShippingMethod,
                requireShipping: true,
                shippingAddressId,
                asNoTracking: false,
                cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var order = BuildOrderAggregate(userId, context);
                DeductStock(context);
                ClearCart(context.User, context.CartLines);

                await _orderRepository.AddEntityAsync(order, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return _mapper.Map<CheckoutResultDto>(order);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task<CheckoutContext> BuildCheckoutContextAsync(
            Guid userId,
            IReadOnlyList<Guid> listItems,
            string? orderPromotionCode,
            string? shippingPromotionCode,
            string? shippingMethod,
            bool requireShipping,
            Guid? shippingAddressId,
            bool asNoTracking,
            CancellationToken cancellationToken)
        {
            var selectedItems = listItems.Distinct().ToList();
            if (selectedItems.Count == 0)
            {
                throw new ValidationException(ErrorCodes.Checkout.EmptySelection, ErrorMessages.Checkout.EmptySelection);
            }

            var user = await _userRepository.GetByIdWithCheckoutDataAsync(userId, asNoTracking, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var cartLines = user.UserCarts
                .Where(c => selectedItems.Contains(c.ProductId))
                .ToList();
            if (cartLines.Count == 0)
            {
                throw new NotFoundException(ErrorCodes.Cart.ItemNotFound, ErrorMessages.Cart.ItemNotFound);
            }

            var selectedProductIds = cartLines
                .Select(c => c.ProductId)
                .Distinct()
                .ToList();

            var products = asNoTracking
                ? await _productRepository.GetByIdsForCartAsync(selectedProductIds, cancellationToken)
                : await _productRepository.GetByIdsForCheckoutAsync(selectedProductIds, cancellationToken);

            var productById = products.ToDictionary(p => p.Id);
            if (productById.Count != selectedProductIds.Count)
            {
                throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Product.ProductNotFound);
            }

            foreach (var cartLine in cartLines)
            {
                ValidateStock(cartLine, productById[cartLine.ProductId]);
            }

            MethodShipping? shipping = null;
            UserAddress? shippingAddress = null;
            if (requireShipping)
            {
                if (string.IsNullOrWhiteSpace(shippingMethod))
                {
                    throw new ValidationException(ErrorCodes.Checkout.ShippingMethodNotFound, ErrorMessages.Checkout.ShippingMethodNotFound);
                }

                shipping = await _methodShippingRepository.GetActiveByCodeAsync(shippingMethod, cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.Checkout.ShippingMethodNotFound, ErrorMessages.Checkout.ShippingMethodNotFound);

                if (!shippingAddressId.HasValue)
                {
                    throw new ValidationException(ErrorCodes.Checkout.ShippingAddressRequired, ErrorMessages.Checkout.ShippingAddressRequired);
                }

                shippingAddress = user.UserAddresses.FirstOrDefault(x => x.Id == shippingAddressId.Value)
                    ?? throw new NotFoundException(ErrorCodes.Checkout.ShippingAddressNotFound, ErrorMessages.Checkout.ShippingAddressNotFound);
            }

            var promotions = await LoadPromotionsAsync(orderPromotionCode, shippingPromotionCode, cancellationToken);
            var result = Calculate(cartLines, productById, shipping?.Price ?? 0m, promotions.orderPromotion, promotions.shippingPromotion);

            return new CheckoutContext(
                user,
                cartLines,
                productById,
                shipping,
                shippingAddress,
                promotions.orderPromotion,
                promotions.shippingPromotion,
                result);
        }

        private async Task<(Promotion? orderPromotion, Promotion? shippingPromotion)> LoadPromotionsAsync(
            string? orderPromotionCode,
            string? shippingPromotionCode,
            CancellationToken cancellationToken)
        {
            var normalizedOrderCode = NormalizeCode(orderPromotionCode);
            var normalizedShippingCode = NormalizeCode(shippingPromotionCode);

            if (normalizedOrderCode is not null &&
                normalizedShippingCode is not null &&
                normalizedOrderCode.Equals(normalizedShippingCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(ErrorCodes.Checkout.DuplicatePromotionType, ErrorMessages.Checkout.DuplicatePromotionType);
            }

            var codes = new List<string>();
            if (normalizedOrderCode is not null) codes.Add(normalizedOrderCode);
            if (normalizedShippingCode is not null) codes.Add(normalizedShippingCode);

            var promotions = await _promotionRepository.GetActiveByCodesAsync(codes, cancellationToken);
            var byCode = promotions.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

            Promotion? orderPromotion = null;
            if (normalizedOrderCode is not null)
            {
                if (!byCode.TryGetValue(normalizedOrderCode, out orderPromotion) || orderPromotion.IsShippingDiscount)
                {
                    throw new ValidationException(ErrorCodes.Checkout.InvalidOrderPromotionCode, ErrorMessages.Checkout.InvalidOrderPromotionCode);
                }
            }

            Promotion? shippingPromotion = null;
            if (normalizedShippingCode is not null)
            {
                if (!byCode.TryGetValue(normalizedShippingCode, out shippingPromotion) || !shippingPromotion.IsShippingDiscount)
                {
                    throw new ValidationException(ErrorCodes.Checkout.InvalidShippingPromotionCode, ErrorMessages.Checkout.InvalidShippingPromotionCode);
                }
            }

            return (orderPromotion, shippingPromotion);
        }

        private static CheckoutCalculationResult Calculate(
            IReadOnlyList<UserCart> cartLines,
            IReadOnlyDictionary<Guid, Product> productById,
            decimal shippingFee,
            Promotion? orderPromotion,
            Promotion? shippingPromotion)
        {
            var itemsSubtotal = cartLines.Sum(c => productById[c.ProductId].RealPrice * c.Quantity);
            var orderDiscount = ComputeDiscount(itemsSubtotal, orderPromotion);
            var shippingDiscount = ComputeDiscount(shippingFee, shippingPromotion);

            var grandTotal = itemsSubtotal - orderDiscount + shippingFee - shippingDiscount;
            if (grandTotal < 0m)
            {
                grandTotal = 0m;
            }

            return new CheckoutCalculationResult(
                itemsSubtotal,
                shippingFee,
                orderDiscount,
                shippingDiscount,
                grandTotal);
        }

        private static decimal ComputeDiscount(decimal amount, Promotion? promotion)
        {
            if (promotion is null || amount <= 0m)
            {
                return 0m;
            }

            var discount = promotion.Type.Equals("Percent", StringComparison.OrdinalIgnoreCase)
                ? amount * (promotion.DiscountAmount / 100m)
                : promotion.DiscountAmount;

            discount = Math.Max(discount, 0m);
            return Math.Min(discount, amount);
        }

        private static void ValidateStock(UserCart cartLine, Product product)
        {
            var stock = product.ProductStock;
            if (stock is null || cartLine.Quantity > stock.Quantity)
            {
                throw new ValidationException(ErrorCodes.Cart.InsufficientStock, ErrorMessages.Cart.InsufficientStock);
            }
        }

        private static string? NormalizeCode(string? code)
        {
            return string.IsNullOrWhiteSpace(code) ? null : code.Trim();
        }

        private static PreCheckoutDto ToPreCheckoutDto(CheckoutContext context)
        {
            return new PreCheckoutDto
            {
                ItemsSubtotal = context.Result.ItemsSubtotal,
                ShippingFee = context.Result.ShippingFee,
                ProductDiscountAmount = context.Result.ProductDiscountAmount,
                ShippingDiscountAmount = context.Result.ShippingDiscountAmount,
                GrandTotal = context.Result.GrandTotal,
                AppliedOrderPromotionCode = context.OrderPromotion?.Code,
                AppliedShippingPromotionCode = context.ShippingPromotion?.Code
            };
        }

        private static Order BuildOrderAggregate(Guid userId, CheckoutContext context)
        {
            var now = DateTime.UtcNow;
            var order = new Order
            {
                UserId = userId,
                SubTotal = context.Result.ItemsSubtotal,
                Total = context.Result.GrandTotal,
                Tax = 0m,
                ViewCartTotal = context.Result.ItemsSubtotal,
                TotalQuantity = context.CartLines.Sum(x => x.Quantity),
                Type = "Online",
                Status = OrderStatus.Pending,
                PaymentStatus = "Unpaid",
                OrderCode = $"ORD-{now:yyyyMMdd}-{Guid.NewGuid():N}"[..22],
                ProductDiscountAmount = context.Result.ProductDiscountAmount,
                ShippingDiscountAmount = context.Result.ShippingDiscountAmount,
                OrderShipping = new OrderShipping
                {
                    FullName = context.ShippingAddress!.FullName,
                    Phone = context.ShippingAddress.Phone,
                    Email = context.ShippingAddress.Email,
                    Province = context.ShippingAddress.Province,
                    District = context.ShippingAddress.District,
                    Address = context.ShippingAddress.Address,
                    ShippingMethodCode = context.ShippingMethod!.Code,
                    ShippingMethodName = context.ShippingMethod.Name,
                    ShippingPrice = context.Result.ShippingFee
                }
            };

            foreach (var line in context.CartLines)
            {
                var product = context.ProductById[line.ProductId];
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    IdCloneProduct = (int)product.IdClone,
                    Name = product.Name,
                    Quantity = line.Quantity,
                    Price = product.Price,
                    RealPrice = product.RealPrice,
                    TotalPrice = product.RealPrice * line.Quantity,
                    RatingAverage = (float)product.RatingAverage,
                    ReviewCount = product.ReviewCount,
                    ThumbnailUrl = product.ThumbnailUrl,
                    Slug = product.Slug
                });
            }

            if (context.OrderPromotion is not null)
            {
                order.OrderPromotions.Add(ToOrderPromotionSnapshot(
                    context.OrderPromotion,
                    isShippingDiscount: false,
                    discountAmount: context.Result.ProductDiscountAmount));
            }

            if (context.ShippingPromotion is not null)
            {
                order.OrderPromotions.Add(ToOrderPromotionSnapshot(
                    context.ShippingPromotion,
                    isShippingDiscount: true,
                    discountAmount: context.Result.ShippingDiscountAmount));
            }

            return order;
        }

        private static OrderPromotion ToOrderPromotionSnapshot(Promotion promotion, bool isShippingDiscount, decimal discountAmount)
        {
            return new OrderPromotion
            {
                PromotionCode = promotion.Code,
                IsShippingDiscount = isShippingDiscount,
                Type = promotion.Type,
                Value = promotion.DiscountAmount,
                DiscountAmount = discountAmount
            };
        }

        private static void DeductStock(CheckoutContext context)
        {
            foreach (var cartLine in context.CartLines)
            {
                var stock = context.ProductById[cartLine.ProductId].ProductStock;
                stock.Quantity -= cartLine.Quantity;
            }
        }

        private static void ClearCart(User user, IReadOnlyList<UserCart> cartLines)
        {
            foreach (var line in cartLines)
            {
                user.UserCarts.Remove(line);
            }
        }

        private sealed record CheckoutCalculationResult(
            decimal ItemsSubtotal,
            decimal ShippingFee,
            decimal ProductDiscountAmount,
            decimal ShippingDiscountAmount,
            decimal GrandTotal);

        private sealed record CheckoutContext(
            User User,
            IReadOnlyList<UserCart> CartLines,
            IReadOnlyDictionary<Guid, Product> ProductById,
            MethodShipping? ShippingMethod,
            UserAddress? ShippingAddress,
            Promotion? OrderPromotion,
            Promotion? ShippingPromotion,
            CheckoutCalculationResult Result);
    }
}

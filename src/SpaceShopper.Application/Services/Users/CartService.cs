using Microsoft.Extensions.Logging;
using AutoMapper;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Requests.Users;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Services.Users
{
    public sealed class CartService(
        IUserRepository userRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CartService> logger) : ICartService
    {
        private static readonly TimeSpan CartCacheTtl = TimeSpan.FromMinutes(2);

        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartService> _logger = logger;

        public async Task<CartDto> GetCartAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.UsersCart(userId);
            var cacheUnavailable = false;
            var cacheHit = false;
            CartDto? cached = null;

            try
            {
                (cacheHit, cached) = await _cacheService.TryGetValueAsync<CartDto>(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                cacheUnavailable = true;
                _logger.LogWarning(ex, "Cart cache unavailable for user {UserId}, falling back to database.", userId);
            }

            if (cacheHit && cached is not null)
            {
                _logger.LogDebug("Cart cache hit for user {UserId}", userId);
                return cached;
            }

            _logger.LogDebug("Cart cache miss for user {UserId}", userId);

            var dto = await LoadCartDtoFromDatabaseAsync(userId, cancellationToken);

            if (!cacheUnavailable)
            {
                try
                {
                    await _cacheService.SetAsync(cacheKey, dto, CartCacheTtl, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set cart cache for user {UserId}", userId);
                }
            }

            _logger.LogInformation("Cart loaded for user {UserId}, line count {Count}", userId, dto.ListItems.Count);
            return dto;
        }

        public async Task<CartDto> UpdateCartQuantityAsync(
            Guid userId,
            Guid productId,
            UpdateCartQuantityRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.Quantity == 0)
            {
                var userForRemove = await _userRepository.GetByIdWithCartAsync(userId, asNoTracking: false, cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

                try
                {
                    userForRemove.RemoveCartItem(productId);
                }
                catch (KeyNotFoundException)
                {
                    throw new NotFoundException(ErrorCodes.Cart.ItemNotFound, ErrorMessages.Cart.ItemNotFound);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await InvalidateCartCacheAsync(userId, cancellationToken);
                _logger.LogInformation(
                    "User {UserId} removed product {ProductId} from cart via PATCH quantity zero",
                    userId,
                    productId);

                return await LoadCartDtoFromDatabaseAsync(userId, cancellationToken);
            }

            var products = await _productRepository.GetByIdsForCartAsync(new[] { productId }, cancellationToken);
            var product = products.FirstOrDefault()
                ?? throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Product.ProductNotFound);

            ValidateProductStockForCartLine(request.Quantity, product);

            var user = await _userRepository.GetByIdWithCartAsync(userId, asNoTracking: false, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            user.UpsertCartItem(productId, request.Quantity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCartCacheAsync(userId, cancellationToken);

            _logger.LogInformation(
                "User {UserId} upserted cart line for product {ProductId} with quantity {Quantity}",
                userId,
                productId,
                request.Quantity);

            return await LoadCartDtoFromDatabaseAsync(userId, cancellationToken);
        }

        public async Task RemoveCartItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithCartAsync(userId, asNoTracking: false, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                user.RemoveCartItem(productId);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException(ErrorCodes.Cart.ItemNotFound, ErrorMessages.Cart.ItemNotFound);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCartCacheAsync(userId, cancellationToken);

            _logger.LogInformation("User {UserId} removed product {ProductId} from cart", userId, productId);
        }

        private async Task InvalidateCartCacheAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await _cacheService.RemoveAsync(CacheKeys.UsersCart(userId), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove cart cache for user {UserId}", userId);
            }
        }

        private async Task<CartDto> LoadCartDtoFromDatabaseAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithCartAsync(userId, asNoTracking: true, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            return await BuildCartDtoAsync(user.UserCarts, cancellationToken);
        }

        private async Task<CartDto> BuildCartDtoAsync(ICollection<UserCart> cartLines, CancellationToken cancellationToken)
        {
            if (cartLines.Count == 0)
            {
                return new CartDto
                {
                    SubTotal = 0m,
                    TotalQuantity = 0,
                    ListItems = Array.Empty<CartItemDto>()
                };
            }

            var ids = cartLines.Select(c => c.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsForCartAsync(ids, cancellationToken);
            var byId = products.ToDictionary(p => p.Id);

            var missing = ids.Count - byId.Count;
            if (missing > 0)
            {
                _logger.LogWarning("Cart has {Missing} reference(s) to missing or deleted products", missing);
            }

            var ordered = cartLines
                .Where(c => byId.ContainsKey(c.ProductId))
                .OrderByDescending(c => c.LastModifiedAt)
                .ToList();

            var itemDtos = ordered
                .Select(c => _mapper.Map<CartItemDto>((c, byId[c.ProductId])))
                .ToList();

            var subTotal = itemDtos.Sum(i => i.Product.RealPrice * i.Quantity);
            var totalQuantity = itemDtos.Sum(i => i.Quantity);

            return new CartDto
            {
                SubTotal = subTotal,
                TotalQuantity = totalQuantity,
                ListItems = itemDtos
            };
        }

        private static void ValidateProductStockForCartLine(int quantity, Product product)
        {
            var stock = product.ProductStock;
            if (stock is null)
            {
                throw new ValidationException(ErrorCodes.Cart.InsufficientStock, ErrorMessages.Cart.InsufficientStock);
            }

            if (quantity > stock.Quantity)
            {
                throw new ValidationException(ErrorCodes.Cart.InsufficientStock, ErrorMessages.Cart.InsufficientStock);
            }

            if (quantity < stock.MinSaleQty)
            {
                throw new ValidationException(
                    ErrorCodes.Application.Validation,
                    $"Quantity must be at least {stock.MinSaleQty}.");
            }

            if (stock.MaxSaleQty > 0 && quantity > stock.MaxSaleQty)
            {
                throw new ValidationException(
                    ErrorCodes.Application.Validation,
                    $"Quantity must not exceed {stock.MaxSaleQty}.");
            }
        }
    }
}

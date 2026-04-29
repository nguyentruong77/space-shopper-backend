using AutoMapper;
using Microsoft.Extensions.Logging;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Orders;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Interfaces.Security;
using SpaceShopper.Application.Requests.Catalog;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Services.Catalog
{
    public sealed class ReviewService(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ICacheKeyHashService cacheKeyHashService,
        IMapper mapper,
        ILogger<ReviewService> logger) : IReviewService
    {
        private static readonly TimeSpan ReviewsCacheTtl = TimeSpan.FromMinutes(3);
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ICacheKeyHashService _cacheKeyHashService = cacheKeyHashService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ReviewService> _logger = logger;

        public async Task<ProductReviewListDto> GetReviewsByProductAsync(
            GetReviewsByProductRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateGetRequest(request);
            var sortBy = request.NormalizeSortValue();
            var cacheUnavailable = false;
            var queryHash = _cacheKeyHashService.Hash(request.ToString());
            var cacheKey = CacheKeys.CatalogProductReviews(request.ProductId, queryHash);

            try
            {
                var (cacheHit, cachedValue) = await _cacheService.TryGetValueAsync<ProductReviewListDto>(cacheKey, cancellationToken);
                if (cacheHit && cachedValue is not null)
                {
                    _logger.LogInformation(
                        "Product reviews cache hit. ProductId: {ProductId}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, Count: {Count}",
                        request.ProductId,
                        request.Page,
                        request.PageSize,
                        sortBy,
                        cachedValue.Reviews.TotalItems);
                    return cachedValue;
                }
            }
            catch (Exception ex)
            {
                cacheUnavailable = true;
                _logger.LogWarning(ex, "Product reviews cache unavailable for ProductId: {ProductId}", request.ProductId);
            }

            var product = await _productRepository.GetByIdWithReviewsAsync(request.ProductId, asNoTracking: true, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Product.ProductNotFound);
            }

            var activeReviews = ApplySort(product.ProductReviews.Where(x => !x.IsDeleted), sortBy).ToList();
            var totalItems = activeReviews.Count;
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)request.PageSize);
            var items = activeReviews
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = new ProductReviewListDto
            {
                Reviews = new PagedResult<ProductReviewDto>
                {
                    Items = _mapper.Map<IReadOnlyList<ProductReviewDto>>(items),
                    CurrentPage = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasNextPage = request.Page < totalPages,
                    HasPreviousPage = request.Page > 1
                },
                Summary = new ReviewSummaryDto
                {
                    AverageRating = product.RatingAverage,
                    TotalReviews = product.ReviewCount
                }
            };

            _logger.LogInformation(
                "Get reviews completed. ProductId: {ProductId}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, Count: {Count}",
                request.ProductId,
                request.Page,
                request.PageSize,
                sortBy,
                result.Reviews.TotalItems);

            if (!cacheUnavailable)
            {
                try
                {
                    await _cacheService.SetAsync(cacheKey, result, ReviewsCacheTtl, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set product reviews cache for ProductId: {ProductId}", request.ProductId);
                }
            }

            return result;
        }

        public async Task<AddProductReviewResultDto> AddReviewAsync(
            Guid userId,
            AddReviewRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateAddRequest(request);
            var product = await _productRepository.GetByIdWithReviewsAsync(request.ProductId, asNoTracking: false, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Add review failed because product not found. UserId: {UserId}, ProductId: {ProductId}", userId, request.ProductId);
                throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Product.ProductNotFound);
            }

            var orderLine = await _orderRepository.GetFirstReviewableOrderDetailAsync(userId, request.ProductId, cancellationToken);
            if (orderLine is null)
            {
                _logger.LogWarning(
                    "Add review denied: no finished order with an unreviewed line for product. UserId: {UserId}, ProductId: {ProductId}",
                    userId,
                    request.ProductId);
                throw new ValidationException(ErrorCodes.Review.ProductNotPurchased, ErrorMessages.Review.ProductNotPurchased);
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
            }

            ProductReview review;
            try
            {
                review = product.AddReview(userId, user.Name, user.Avatar, request.Rating, request.Comment, orderLine.OrderId);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException(ErrorCodes.Review.RatingOutOfRange, ErrorMessages.Review.RatingOutOfRange);
            }
            catch (ArgumentException)
            {
                throw new ValidationException(ErrorCodes.Review.CommentRequired, ErrorMessages.Review.CommentRequired);
            }

            orderLine.IsReviewed = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateReviewCachesAsync(request.ProductId, cancellationToken);

            _logger.LogInformation(
                "Review added successfully. UserId: {UserId}, ProductId: {ProductId}, Rating: {Rating}",
                userId,
                request.ProductId,
                request.Rating);

            return new AddProductReviewResultDto
            {
                Review = _mapper.Map<ProductReviewDto>(review),
                Summary = new ReviewSummaryDto
                {
                    AverageRating = product.RatingAverage,
                    TotalReviews = product.ReviewCount
                }
            };
        }

        private static IEnumerable<ProductReview> ApplySort(IEnumerable<ProductReview> reviews, string sortBy)
        {
            return sortBy switch
            {
                "rating_desc" => reviews.OrderByDescending(x => x.Star).ThenByDescending(x => x.CreatedOn),
                "rating_asc" => reviews.OrderBy(x => x.Star).ThenByDescending(x => x.CreatedOn),
                _ => reviews.OrderByDescending(x => x.CreatedOn)
            };
        }

        private static void ValidateGetRequest(GetReviewsByProductRequest request)
        {
            if (request.ProductId == Guid.Empty)
            {
                throw new ValidationException(ErrorCodes.Application.Validation, "ProductId is required.");
            }

            if (request.Page <= 0 || request.PageSize <= 0)
            {
                throw new ValidationException(ErrorCodes.Review.InvalidPaging, ErrorMessages.Review.InvalidPaging);
            }

            if (request.PageSize > 100)
            {
                request.PageSize = 100;
            }

            var sortBy = request.NormalizeSortValue();
            if (sortBy is not ("latest" or "rating_desc" or "rating_asc"))
            {
                throw new ValidationException(ErrorCodes.Review.InvalidSortBy, ErrorMessages.Review.InvalidSortBy);
            }
        }

        private static void ValidateAddRequest(AddReviewRequest request)
        {
            if (request.ProductId == Guid.Empty)
            {
                throw new ValidationException(ErrorCodes.Application.Validation, "ProductId is required.");
            }

            if (request.Rating is < 1 or > 5)
            {
                throw new ValidationException(ErrorCodes.Review.RatingOutOfRange, ErrorMessages.Review.RatingOutOfRange);
            }

            if (string.IsNullOrWhiteSpace(request.Comment))
            {
                throw new ValidationException(ErrorCodes.Review.CommentRequired, ErrorMessages.Review.CommentRequired);
            }
        }

        private async Task InvalidateReviewCachesAsync(Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                await _cacheService.RemoveAsync(CacheKeys.CatalogProductDetail(productId), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove product detail cache for ProductId: {ProductId}", productId);
            }
        }
    }
}

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
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Services.Users
{
    public sealed class WishlistService(
        IUserRepository userRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<WishlistService> logger) : IWishlistService
    {
        private static readonly TimeSpan WishlistCacheTtl = TimeSpan.FromMinutes(2);

        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<WishlistService> _logger = logger;

        public async Task<IReadOnlyList<WishlistItemDto>> GetWishlistAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.UsersWishlist(userId);
            var cacheUnavailable = false;
            var cacheHit = false;
            List<WishlistItemDto>? cached = null;

            try
            {
                (cacheHit, cached) = await _cacheService.TryGetValueAsync<List<WishlistItemDto>>(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                cacheUnavailable = true;
                _logger.LogWarning(ex, "Wishlist cache unavailable for user {UserId}, falling back to database.", userId);
            }

            if (cacheHit && cached is not null)
            {
                _logger.LogDebug("Wishlist cache hit for user {UserId}, item count {Count}", userId, cached.Count);
                return cached;
            }

            _logger.LogDebug("Wishlist cache miss for user {UserId}", userId);

            var user = await _userRepository.GetByIdWithWishlistAsync(userId, asNoTracking: true, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var dtos = await BuildWishlistDtosAsync(user.WishlistItems, cancellationToken);

            if (!cacheUnavailable)
            {
                try
                {
                    await _cacheService.SetAsync(cacheKey, dtos.ToList(), WishlistCacheTtl, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set wishlist cache for user {UserId}", userId);
                }
            }

            _logger.LogInformation("Wishlist loaded for user {UserId}, item count {Count}", userId, dtos.Count);
            return dtos;
        }

        public async Task<WishlistItemDto> AddWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Product.ProductNotFound);

            var user = await _userRepository.GetByIdWithWishlistAsync(userId, asNoTracking: false, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                var item = user.AddToWishlist(productId);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await InvalidateWishlistCacheAsync(userId, cancellationToken);

                _logger.LogInformation("User {UserId} added product {ProductId} to wishlist", userId, productId);
                return _mapper.Map<WishlistItemDto>((item, product));
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("Duplicate wishlist add for user {UserId}, product {ProductId}", userId, productId);
                throw new ValidationException(ErrorCodes.Wishlist.Duplicate, ErrorMessages.Wishlist.Duplicate);
            }
        }

        public async Task RemoveWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithWishlistAsync(userId, asNoTracking: false, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                user.RemoveFromWishlist(productId);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException(ErrorCodes.Wishlist.ItemNotFound, ErrorMessages.Wishlist.ItemNotFound);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateWishlistCacheAsync(userId, cancellationToken);
            _logger.LogInformation("User {UserId} removed product {ProductId} from wishlist", userId, productId);
        }

        private async Task InvalidateWishlistCacheAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await _cacheService.RemoveAsync(CacheKeys.UsersWishlist(userId), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove wishlist cache for user {UserId}", userId);
            }
        }

        private async Task<IReadOnlyList<WishlistItemDto>> BuildWishlistDtosAsync(
            ICollection<WishlistItem> wishlistItems,
            CancellationToken cancellationToken)
        {
            var ids = wishlistItems.Select(w => w.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsForWishlistAsync(ids, cancellationToken);
            var byId = products.ToDictionary(p => p.Id);

            var missing = ids.Count - byId.Count;
            if (missing > 0)
            {
                _logger.LogWarning("Wishlist has {Missing} reference(s) to missing or deleted products", missing);
            }

            var ordered = wishlistItems
                .Where(w => byId.ContainsKey(w.ProductId))
                .OrderBy(w => byId[w.ProductId].Name)
                .ToList();

            return ordered
                .Select(w => _mapper.Map<WishlistItemDto>((w, byId[w.ProductId])))
                .ToList();
        }
    }
}

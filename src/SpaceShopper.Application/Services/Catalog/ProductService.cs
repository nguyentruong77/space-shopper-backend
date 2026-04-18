using AutoMapper;
using Microsoft.Extensions.Logging;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Interfaces.Security;
using SpaceShopper.Application.Requests.Catalog;

namespace SpaceShopper.Application.Services.Catalog
{
    public sealed class ProductService(
        IProductRepository productRepository,
        ICacheService cacheService,
        ICacheKeyHashService cacheKeyHashService,
        IMapper mapper,
        ILogger<ProductService> logger) : IProductService
    {
        private static readonly TimeSpan SearchCacheTtl = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan ProductDetailCacheTtl = TimeSpan.FromMinutes(20);
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ICacheKeyHashService _cacheKeyHashService = cacheKeyHashService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductService> _logger = logger;

        public async Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default)
        {
            NormalizePaging(request);
            var cacheKey = CacheKeys.CatalogProductsSearch(_cacheKeyHashService.Hash(request.ToString()));
            var cacheUnavailable = false;
            var cacheHit = false;
            PagedResult<ProductListItemDto>? cachedResult = null;

            try
            {
                (cacheHit, cachedResult) = await _cacheService.TryGetValueAsync<PagedResult<ProductListItemDto>>(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                cacheUnavailable = true;
                _logger.LogWarning(ex, "Product search cache unavailable, fallback to database query.");
            }

            if (cacheHit && cachedResult is not null)
            {
                _logger.LogInformation(
                    "Product search cache hit. Keyword: {Keyword}, CategoryId: {CategoryId}, Page: {Page}, PageSize: {PageSize}, Sort: {Sort}",
                    request.Keyword,
                    request.CategoryId,
                    request.Page,
                    request.PageSize,
                    request.NormalizeSortValue());
                return cachedResult;
            }

            _logger.LogInformation(
                "Product search cache miss. Keyword: {Keyword}, CategoryId: {CategoryId}, Page: {Page}, PageSize: {PageSize}, Sort: {Sort}",
                request.Keyword,
                request.CategoryId,
                request.Page,
                request.PageSize,
                request.NormalizeSortValue());

            var (items, totalItems) = await _productRepository.GetListProductByQueryAsync(request, cancellationToken);
            var itemDtos = _mapper.Map<IReadOnlyList<ProductListItemDto>>(items);
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var result = new PagedResult<ProductListItemDto>
            {
                Items = itemDtos,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };

            if (!cacheUnavailable)
            {
                try
                {
                    await _cacheService.SetAsync(cacheKey, result, SearchCacheTtl, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Product search cache set failed.");
                }
            }

            return result;
        }

        public async Task<ProductDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.CatalogProductDetail(id);
            var cacheHit = false;
            ProductDetailDto? cachedResult = null;

            try
            {
                (cacheHit, cachedResult) = await _cacheService.TryGetValueAsync<ProductDetailDto>(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Product detail cache unavailable for ProductId: {ProductId}.", id);
            }

            if (cacheHit && cachedResult is not null)
            {
                _logger.LogInformation("Product detail cache hit for ProductId: {ProductId}", id);
                return cachedResult;
            }

            _logger.LogInformation("Product detail cache miss for ProductId: {ProductId}", id);

            var product = await _productRepository.GetDetailByIdAsync(id, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Product not found. ProductId: {ProductId}", id);
                throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Product.ProductNotFound);
            }

            var result = _mapper.Map<ProductDetailDto>(product);

            try
            {
                await _cacheService.SetAsync(cacheKey, result, ProductDetailCacheTtl, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Product detail cache set failed for ProductId: {ProductId}.", id);
            }

            return result;
        }

        private static void NormalizePaging(ProductSearchRequest request)
        {
            if (request.Page <= 0)
            {
                request.Page = 1;
            }

            if (request.PageSize <= 0)
            {
                request.PageSize = 15;
            }
            else if (request.PageSize > 100)
            {
                request.PageSize = 100;
            }
        }
    }
}

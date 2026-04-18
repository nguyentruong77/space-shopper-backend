using AutoMapper;
using Microsoft.Extensions.Logging;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;

namespace SpaceShopper.Application.Services.Catalog
{
    public sealed class CategoryService(
        ICategoryRepository categoryRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CategoryService> logger) : ICategoryService
    {
        private static readonly TimeSpan CategoriesListCacheTtl = TimeSpan.FromMinutes(45);
        private static readonly TimeSpan CategoryDetailCacheTtl = TimeSpan.FromMinutes(45);
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CategoryService> _logger = logger;

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.CatalogCategoriesAll();
            var cacheUnavailable = false;
            var cacheHit = false;
            List<CategoryDto>? cachedResult = null;

            try
            {
                (cacheHit, cachedResult) = await _cacheService.TryGetValueAsync<List<CategoryDto>>(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                cacheUnavailable = true;
                _logger.LogWarning(ex, "Category list cache unavailable, fallback to database query.");
            }

            if (cacheHit && cachedResult is not null)
            {
                _logger.LogInformation(
                    "Category list cache hit. Count: {Count}",
                    cachedResult.Count);
                return cachedResult;
            }

            _logger.LogInformation("Category list cache miss.");

            var entities = await _categoryRepository.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<List<CategoryDto>>(entities);

            _logger.LogInformation("Category list loaded from database. Count: {Count}", dtos.Count);

            if (!cacheUnavailable)
            {
                try
                {
                    await _cacheService.SetAsync(cacheKey, dtos, CategoriesListCacheTtl, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Category list cache set failed.");
                }
            }

            return dtos;
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.CatalogCategoryDetail(id);
            var cacheHit = false;
            CategoryDto? cachedResult = null;

            try
            {
                (cacheHit, cachedResult) = await _cacheService.TryGetValueAsync<CategoryDto>(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Category detail cache unavailable for CategoryId: {CategoryId}.", id);
            }

            if (cacheHit && cachedResult is not null)
            {
                _logger.LogInformation("Category detail cache hit for CategoryId: {CategoryId}", id);
                return cachedResult;
            }

            _logger.LogInformation("Category detail cache miss for CategoryId: {CategoryId}", id);

            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category is null)
            {
                _logger.LogWarning("Category not found. CategoryId: {CategoryId}", id);
                throw new NotFoundException(ErrorCodes.Application.NotFound, ErrorMessages.Category.CategoryNotFound);
            }

            var result = _mapper.Map<CategoryDto>(category);

            try
            {
                await _cacheService.SetAsync(cacheKey, result, CategoryDetailCacheTtl, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Category detail cache set failed for CategoryId: {CategoryId}.", id);
            }

            return result;
        }
    }
}

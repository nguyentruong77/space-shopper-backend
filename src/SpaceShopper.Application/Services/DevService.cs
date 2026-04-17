using Microsoft.AspNetCore.WebUtilities;
using SpaceShopper.Application.Dtos.Dev;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Domain.Entities.Catalog;
using System.Net.Http.Json;

namespace SpaceShopper.Application.Services
{
    public class DevService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public DevService(HttpClient httpClient, IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _httpClient = httpClient;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ImportCategoryAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExternalCategoryResponse>("/product/categories");
                var categories = response?.data ?? new List<CloneCategoryDto>();
                if (categories.Count == 0)
                {
                    return true;
                }

                var existingCategories = await _categoryRepository.GetAllAsync();
                var categoryCloneIdSet = existingCategories
                    .Select(c => c.IdClone)
                    .ToHashSet();

                var categoriesToImport = categories
                    .Where(dto => !categoryCloneIdSet.Contains(dto.id))
                    .ToList();

                if (categoriesToImport.Count == 0)
                {
                    return true;
                }

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    foreach (var dto in categoriesToImport)
                    {
                        var category = new Category
                        {
                            IdClone = dto.id,
                            Title = dto.title ?? string.Empty,
                            Slug = dto.slug,
                            Status = dto.status,
                            Position = dto.position,
                            ParentId = Guid.Empty,
                            CreatedOn = DateTime.UtcNow
                        };

                        await _categoryRepository.AddEntityAsync(category);
                    }

                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while importing categories: {ex}");
                return false;
            }
        }

        public async Task<bool> ImportProductAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                if (categories.Count == 0)
                {
                    return false;
                }

                var categoryIdByCloneId = categories.ToDictionary(c => c.IdClone, c => c.Id);
                var totalPage = await GetTotalPageAsync();
                if (totalPage <= 0)
                {
                    return true;
                }

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    for (var page = 1; page <= totalPage; page++)
                    {
                        var products = await GetProductPageAsync(page);
                        if (products.Count == 0)
                        {
                            continue;
                        }

                        foreach (var dto in products)
                        {
                            if (!categoryIdByCloneId.TryGetValue(dto.categories, out var categoryId))
                            {
                                continue;
                            }

                            if (await _productRepository.IsExistAsync(dto.id))
                            {
                                continue;
                            }

                            var product = new Product
                            {
                                IdClone = dto.id,
                                CategoryId = categoryId,
                                Name = dto.name ?? string.Empty,
                                Slug = dto.slug,
                                Description = dto.description,
                                ShortDescription = dto.short_description,
                                Price = dto.price,
                                RealPrice = dto.real_price,
                                DiscountRate = dto.discount_rate,
                                RatingAverage = dto.rating_average,
                                ReviewCount = dto.review_count,
                                ThumbnailUrl = dto.thumbnail_url,
                                CreatedOn = DateTime.UtcNow,
                                ProductImages = MapImages(dto.images),
                                ProductStock = MapStock(dto.stock_item)
                            };

                            var reviews = await GetProductReviewsAsync(product.IdClone);
                            foreach (var reviewDto in reviews)
                            {
                                product.ProductReviews.Add(new ProductReview
                                {
                                    ProductId = product.Id,
                                    OrderId = null,
                                    UserId = null,
                                    CreatedOn = DateTimeOffset.FromUnixTimeMilliseconds(reviewDto.createdAt).UtcDateTime,
                                    Content = reviewDto.content ?? string.Empty,
                                    Star = Math.Clamp((int)Math.Round(reviewDto.star), 1, 5),
                                    NameUser = reviewDto.user?.name ?? "Anonymous",
                                    AvtUrl = reviewDto.user?.avatar
                                });
                            }

                            await _productRepository.AddEntityAsync(product);
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }

                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while importing products: {ex}");
                return false;
            }
        }

        private async Task<int> GetTotalPageAsync()
        {
            var query = new Dictionary<string, string?>
            {
                ["fields"] = "id",
                ["limit"] = "100",
                ["page"] = "1"
            };

            var firstPageUrl = QueryHelpers.AddQueryString("/product", query);
            var firstPageResponse = await _httpClient.GetFromJsonAsync<ExternalProductResponse>(firstPageUrl);
            return firstPageResponse?.paginate?.totalPage ?? 0;
        }

        private async Task<List<CloneProductDto>> GetProductPageAsync(int page)
        {
            var query = new Dictionary<string, string?>
            {
                ["fields"] = "id,_id,categories,name,slug,description,short_description,price,real_price,discount_rate,rating_average,review_count,stock_item,images,thumbnail_url",
                ["limit"] = "100",
                ["page"] = page.ToString()
            };

            var pageUrl = QueryHelpers.AddQueryString("/product", query);
            var pageResponse = await _httpClient.GetFromJsonAsync<ExternalProductResponse>(pageUrl);
            return pageResponse?.data ?? new List<CloneProductDto>();
        }

        private async Task<List<CloneReviewProductDto>> GetProductReviewsAsync(long productCloneId)
        {
            var response = await _httpClient.GetFromJsonAsync<ExternalReviewProductResponse>($"/review/{productCloneId}?limit=1000");
            return response?.data ?? new List<CloneReviewProductDto>();
        }

        private static List<ProductImage> MapImages(List<Image>? images)
        {
            if (images is null || images.Count == 0)
            {
                return new List<ProductImage>();
            }

            return images
                .Select((img, index) => new ProductImage
                {
                    Label = img.label,
                    Position = img.position ?? index + 1,
                    BaseUrl = img.base_url,
                    ThumbnailUrl = img.thumbnail_url,
                    SmallUrl = img.small_url,
                    MediumUrl = img.medium_url,
                    LargeUrl = img.large_url,
                    IsGallery = img.is_gallery
                })
                .ToList();
        }

        private static ProductStock MapStock(StockItem? stockItem)
        {
            var minSaleQty = stockItem?.min_sale_qty ?? 1;
            var maxSaleQty = stockItem?.max_sale_qty ?? minSaleQty;
            if (maxSaleQty < minSaleQty)
            {
                maxSaleQty = minSaleQty;
            }

            return new ProductStock
            {
                Quantity = stockItem?.qty ?? 0,
                MinSaleQty = minSaleQty,
                MaxSaleQty = maxSaleQty,
                PreOrder = false
            };
        }
    }
}

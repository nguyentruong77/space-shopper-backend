using SpaceShopper.Application.Dtos.Dev;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.ValueObjects;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using SpaceShopper.Application.Interfaces.IRepositories.Common;

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
                //return true;
                //var res = await _httpClient.GetFromJsonAsync<ExternalCategoryResponse>("/product/categories");
                //var categories = res?.data ?? new List<CloneCategoryDto>();
                //await _unitOfWork.BeginTransactionAsync();
                //int i = 1;
                //foreach (var dto in categories)
                //{
                //    // check trùng theo CloneId
                //    //if (await _categoryRepository.IsExistAsync(dto.id))
                //    //    continue;
                //    var category = new Category();
                //    category.IdClone = dto.id;
                //    category.Title = dto.title ?? string.Empty;
                //    category.Slug = dto.slug;
                //    category.Status = dto.status;
                //    category.Position = dto.position;
                //    category.ParentId = Guid.Empty;
                //    category.CreatedOn = DateTime.UtcNow;
                //    await _categoryRepository.AddAsync(category);
                //    Console.WriteLine($"Imported category: {i++}");
                //}
                //await _unitOfWork.CommitAsync();
                //Console.WriteLine($"Category import completed. Imported total: {i}");
                return false;
            }
            catch (Exception ex)
            {
                // Log exception (logging mechanism not shown here)
                Console.WriteLine("An error occurred while importing products:", ex);
                return false;
            }
        }
        public async Task<bool> ImportProductAsync()
        {
            //395, 394358
            try
            {
                //List<Category> categories = await _categoryRepository.GetAllAsync();
                //for (int i = 1; i <= 395; i++)
                //{
                //    var query = new Dictionary<string, string>
                //    {
                //        ["fields"] = "id,_id,categories,name,slug,description,price,real_price,discount_rate,rating_average,review_count,stock_item,images,thumbnail_url",
                //        ["limit"] = "100",
                //        ["page"] = i.ToString()
                //    };
                //    var url = QueryHelpers.AddQueryString("/product", query);
                //    var res = await _httpClient.GetFromJsonAsync<ExternalProductResponse>(url);
                //    var products = res?.data ?? new List<CloneProductDto>();

                //    await _unitOfWork.BeginTransactionAsync();
                //    foreach (var dto in products)
                //    {
                //        if (!(await _categoryRepository.IsExistAsync(dto?.categories ?? 0)))
                //        {
                //            Console.WriteLine($"Category not found for product IdClone: {dto?.categories}");
                //            continue;
                //        }
                //        var product = new Product();
                //        product.IdClone = dto.id;
                //        product.Name = dto.name ?? string.Empty;
                //        product.Slug = dto.slug;
                //        product.Description = dto.description;
                //        product.ShortDescription = dto.short_description;
                //        product.Price = dto.price;
                //        product.RealPrice = dto.real_price;
                //        product.DiscountRate = dto.discount_rate;
                //        product.RatingAverage = dto.rating_average;
                //        product.ReviewCount = dto.review_count;
                //        product.ThumbnailUrl = dto.thumbnail_url;
                //        product.CreatedOn = DateTime.UtcNow;
                //        product.CategoryId = categories.SingleOrDefault(e => e.IdClone == dto?.categories)?.Id ?? Guid.Empty;

                //        product.ProductImages = new List<ProductImage>();

                //        int position = 1;
                //        foreach (var img in dto.images)
                //        {
                //            var productImage = new ProductImage();
                //            productImage.Label = img.label;
                //            productImage.Position = position++;
                //            productImage.BaseUrl = img.base_url;
                //            productImage.ThumbnailUrl = img.thumbnail_url;
                //            productImage.SmallUrl = img.small_url;
                //            productImage.MediumUrl = img.medium_url;
                //            productImage.LargeUrl = img.large_url;
                //            productImage.IsGallery = img.is_gallery;
                //            product.ProductImages ??= new List<ProductImage>();
                //            product.ProductImages.Add(productImage);
                //        }

                //        var stock = new ProductStock();
                //        stock.ProductId = product.Id;
                //        stock.Quantity = dto.stock_item?.qty ?? 0;
                //        stock.MinSaleQty = dto.stock_item?.min_sale_qty ?? 1;
                //        stock.MaxSaleQty = dto.stock_item?.max_sale_qty ?? 1;
                //        product.ProductStock = stock;

                //        // Clone Review Product
                //        var resReview = await _httpClient.GetFromJsonAsync<ExternalReviewProductResponse>($"/review/{product.IdClone}?limit=1000");
                //        var reviews = resReview?.data ?? new List<CloneReviewProductDto>();
                //        foreach (var reviewDto in reviews)
                //        {
                //            var review = new ProductReview();
                //            review.ProductId = product.Id;
                //            review.OrderId = null;
                //            review.CreatedOn = DateTimeOffset.FromUnixTimeMilliseconds(reviewDto.createdAt).UtcDateTime;
                //            review.Content = reviewDto.content;
                //            review.Star = (int)Math.Round(reviewDto.star);
                //            review.UserId = null;
                //            review.NameUser = reviewDto.user?.name ?? "Anonymous";
                //            review.AvtUrl = reviewDto.user?.avatar;
                //            product.ProductReviews ??= new List<ProductReview>();

                //            product.ProductReviews.Add(review);
                //        }

                //        await _productRepository.AddAsync(product);
                //    }
                //    await _unitOfWork.CommitAsync();
                //    Console.WriteLine($"Category import completed. Imported total: {i*100}");
                //}
                return true;
            }
            catch (Exception ex)
            {
                // Log exception (logging mechanism not shown here)
                Console.WriteLine("An error occurred while importing products:", ex);
                return false;
            }
        }
    }
}

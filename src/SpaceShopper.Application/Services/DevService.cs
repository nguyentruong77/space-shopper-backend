using SpaceShopper.Application.Dtos.Dev;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.ValueObjects;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace SpaceShopper.Application.Services
{
    public class DevService
    {
        private readonly IProductRepository _productRepository;
        private readonly HttpClient _httpClient;

        public DevService(HttpClient httpClient, IProductRepository productRepository)
        {
            _httpClient = httpClient;
            _productRepository = productRepository;
        }

        public async Task<bool> ImportProductAsync()
        {
            var newProducts = new List<Product>();
            var query = new Dictionary<string, string>
            {
                ["fields"] = "name,real_price,price,categories,slug,id,description",
            };

            try
            {
                var url = QueryHelpers.AddQueryString("/product", query);

                var res = await _httpClient.GetFromJsonAsync<ExternalProductResponse>(url);
                var products = res?.data ?? new List<CloneProductDto>();

                foreach (var dto in products)
                {
                    // check trùng theo CloneId
                    if (await _productRepository.ExistsByExternalIdAsync(dto.id))
                        continue;

                    var product = Product.Create(
                        idClone: dto.id,
                        name: dto.name,
                        price: new Money(dto.price, "VND"),
                        realPrice: new Money(dto.real_price, "VND"),
                        description: dto.description,
                        slug: dto.slug
                    );

                    newProducts.Add(product);
                }

                if (newProducts.Any())
                {
                    await _productRepository.AddRangeAsync(newProducts);
                    await _productRepository.SaveChangesAsync();
                    return true;
                }
                return false;
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

using SpaceShopper.Application.Dtos;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.ValueObjects;
using System.Net.Http.Json;

namespace SpaceShopper.Application.Services.Catalog
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(HttpClient httpClient, IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}

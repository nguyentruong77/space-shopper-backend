using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Controllers.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Requests.Catalog;

namespace SpaceShopper.API.Controllers.Catalog
{
    [Route("/api/v1/[controller]")]
    public class ProductController(IProductService productService, IProductRepository productRepository) : BaseController
    {
        private readonly IProductService _productService = productService;
        private readonly IProductRepository _productRepository = productRepository;
        [AllowAnonymous]
        [HttpGet("products")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchRequest query)
        {
            var result = 1;//await _productService.SearchAsync(query);
            return Ok(result);
        }
    }
}

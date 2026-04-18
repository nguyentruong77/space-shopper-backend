using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Requests.Catalog;

namespace SpaceShopper.API.Controllers.Catalog
{
    [ApiController]
    [Route("api/v1/products")]
    public sealed class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductListItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchRequest query, CancellationToken cancellationToken)
        {
            var result = await _productService.SearchAsync(query, cancellationToken);
            return Ok(ApiResponse<PagedResult<ProductListItemDto>>.Ok(result));
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductDetail([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<ProductDetailDto>.Ok(result));
        }
    }
}

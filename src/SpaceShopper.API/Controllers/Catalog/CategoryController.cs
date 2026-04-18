using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;

namespace SpaceShopper.API.Controllers.Catalog
{
    [ApiController]
    [Route("api/v1/categories")]
    public sealed class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(result));
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategory([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<CategoryDto>.Ok(result));
        }
    }
}

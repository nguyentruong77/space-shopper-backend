using Microsoft.AspNetCore.Mvc;
using SpaceShopper.Application.Services;

namespace SpaceShopper.API.Controllers.Dev
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        private readonly DevService _devService;

        public DevController(DevService devService)
        {
            _devService = devService;
        }

        [HttpGet("clone-products")]
        public async Task<IActionResult> ImportProducts()
        {
            var result = await _devService.ImportProductAsync();

            return Ok(result);
        }
        [HttpGet("clone-categories")]
        public async Task<IActionResult> ImportCategories()
        {
            var result = await _devService.ImportCategoryAsync();

            return Ok(result);
        }
    }
}

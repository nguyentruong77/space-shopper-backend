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
        public async Task<IActionResult> Import()
        {
            var result = await _devService.ImportProductAsync();

            return Ok(result);
        }
    }
}

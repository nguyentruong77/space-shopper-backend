using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Shipping;
using SpaceShopper.Application.Interfaces.Iservices.Shipping;

namespace SpaceShopper.API.Controllers.Shipping
{
    [ApiController]
    [Route("api/v1/shippingmethod")]
    public sealed class ShippingMethodController(IShippingMethodService shippingMethodService) : ControllerBase
    {
        private readonly IShippingMethodService _shippingMethodService = shippingMethodService;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShippingMethods(CancellationToken cancellationToken)
        {
            var shippingMethods = await _shippingMethodService.GetShippingMethodsAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<ShippingMethodDto>>.Ok(shippingMethods));
        }
    }
}

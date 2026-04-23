using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Application.Interfaces.Iservices.Orders;
using SpaceShopper.Application.Requests.Orders;

namespace SpaceShopper.API.Controllers.Orders
{
    [ApiController]
    [Route("api/v1/orders")]
    public sealed class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpPost("pre-checkout")]
        [Authorize]
        public async Task<IActionResult> PreCheckout([FromBody] PreCheckoutRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.PreCheckoutAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<PreCheckoutDto>.Ok(result));
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.CheckoutAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<CheckoutResultDto>.Ok(result));
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var id))
            {
                throw new UnauthorizedException(ErrorCodes.Auth.Unauthorized, ErrorMessages.Auth.Unauthorized);
            }

            return id;
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.API.Controllers.Cart
{
    [ApiController]
    [Route("api/v1/cart")]
    public sealed class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetCartAsync(GetCurrentUserId(), cancellationToken);
            return Ok(ApiResponse<CartDto>.Ok(cart));
        }

        [HttpPatch("{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuantity(
            [FromRoute] Guid productId,
            [FromBody] UpdateCartQuantityRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cart = await _cartService.UpdateCartQuantityAsync(GetCurrentUserId(), productId, request, cancellationToken);
            return Ok(ApiResponse<CartDto>.Ok(cart));
        }

        [HttpDelete("{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            await _cartService.RemoveCartItemAsync(GetCurrentUserId(), productId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { deleteCount = 1 }));
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

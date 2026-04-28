using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Controllers.Common;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.API.Controllers.Cart
{
    [ApiController]
    [Route("api/v1/cart")]
    public sealed class CartController(ICartService cartService) : BaseController
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
    }
}

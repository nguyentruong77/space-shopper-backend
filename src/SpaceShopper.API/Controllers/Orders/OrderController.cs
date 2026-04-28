using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Controllers.Common;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Application.Interfaces.Iservices.Orders;
using SpaceShopper.Application.Requests.Orders;

namespace SpaceShopper.API.Controllers.Orders
{
    [ApiController]
    [Route("api/v1/orders")]
    public sealed class OrderController(IOrderService orderService) : BaseController
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] OrderFilterRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrdersAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<PagedResult<OrderListDto>>.Ok(result));
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderDetailAsync(GetCurrentUserId(), id, cancellationToken);
            return Ok(ApiResponse<OrderDetailDto>.Ok(result));
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> GetOrderCountByStatus([FromQuery] OrderStatusCountQuery query, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderCountByStatusAsync(GetCurrentUserId(), query, cancellationToken);
            return Ok(ApiResponse<OrderStatusCountDto>.Ok(result));
        }
    }
}

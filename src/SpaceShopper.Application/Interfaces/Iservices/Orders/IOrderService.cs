using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Requests.Orders;

namespace SpaceShopper.Application.Interfaces.Iservices.Orders
{
    public interface IOrderService
    {
        Task<PreCheckoutDto> PreCheckoutAsync(Guid userId, PreCheckoutRequest request, CancellationToken cancellationToken = default);
        Task<CheckoutResultDto> CheckoutAsync(Guid userId, CheckoutRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<OrderListDto>> GetOrdersAsync(Guid userId, OrderFilterRequest request, CancellationToken cancellationToken = default);
        Task<OrderDetailDto> GetOrderDetailAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
        Task<OrderStatusCountDto> GetOrderCountByStatusAsync(Guid userId, OrderStatusCountQuery query, CancellationToken cancellationToken = default);
    }
}

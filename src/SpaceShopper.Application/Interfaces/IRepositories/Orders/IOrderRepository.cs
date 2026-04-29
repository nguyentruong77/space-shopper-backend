using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Domain.Entities.Orders;
using SpaceShopper.Domain.Enums;

namespace SpaceShopper.Application.Interfaces.IRepositories.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<(IReadOnlyList<Order> Items, int TotalItems)> GetOrdersAsync(
            Guid userId,
            OrderStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<Order?> GetDetailByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<int> CountByStatusAsync(Guid userId, OrderStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Oldest finished order line for this product that is not yet marked reviewed (tracked entity).
        /// </summary>
        Task<OrderDetail?> GetFirstReviewableOrderDetailAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    }
}

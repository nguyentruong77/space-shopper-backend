using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Orders;
using SpaceShopper.Domain.Entities.Orders;
using SpaceShopper.Domain.Enums;
using SpaceShopper.Infrastructure.Common;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Orders
{
    public sealed class OrderRepository(SpaceShopperDbContext context) : Repository<Order>(context), IOrderRepository
    {
        private readonly SpaceShopperDbContext _dbContext = context;

        public async Task<(IReadOnlyList<Order> Items, int TotalItems)> GetOrdersAsync(
            Guid userId,
            OrderStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var baseQuery = _dbContext.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.UserId == userId);

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(o => o.Status == status.Value);
            }

            var totalItems = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderPromotions)
                .Include(o => o.OrderShipping)
                .OrderByDescending(o => o.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalItems);
        }

        public async Task<Order?> GetDetailByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderPromotions)
                .Include(o => o.OrderShipping)
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == orderId, cancellationToken);
        }

        public async Task<int> CountByStatusAsync(Guid userId, OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.UserId == userId && o.Status == status)
                .CountAsync(cancellationToken);
        }

        public async Task<OrderDetail?> GetFirstReviewableOrderDetailAsync(
            Guid userId,
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await (
                from d in _dbContext.OrderDetails
                join o in _dbContext.Orders on d.OrderId equals o.Id
                where d.ProductId == productId
                      && !d.IsReviewed
                      && !o.IsDeleted
                      && o.UserId == userId
                      && o.Status == OrderStatus.Finished
                orderby o.CreatedOn
                select d
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}

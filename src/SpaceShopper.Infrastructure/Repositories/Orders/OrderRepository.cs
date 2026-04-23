using SpaceShopper.Application.Interfaces.IRepositories.Orders;
using SpaceShopper.Domain.Entities.Orders;
using SpaceShopper.Infrastructure.Common;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Orders
{
    public sealed class OrderRepository(SpaceShopperDbContext context) : Repository<Order>(context), IOrderRepository
    {
    }
}

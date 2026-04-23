using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Domain.Entities.Shipping;

namespace SpaceShopper.Application.Interfaces.IRepositories.Shipping
{
    public interface IMethodShippingRepository : IRepository<MethodShipping>
    {
        Task<List<MethodShipping>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<MethodShipping?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}

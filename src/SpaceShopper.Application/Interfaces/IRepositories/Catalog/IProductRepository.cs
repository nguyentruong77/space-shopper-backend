using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Requests.Catalog;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Interfaces.IRepositories.Catalog
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<bool> IsExistAsync(long id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetListProductByQueryAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);
    }
}

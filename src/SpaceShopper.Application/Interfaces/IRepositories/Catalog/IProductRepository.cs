using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Requests.Catalog;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Interfaces.IRepositories.Catalog
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<bool> IsExistAsync(long id, CancellationToken cancellationToken = default);
        Task<Product?> GetDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Product> Items, int TotalItems)> GetListProductByQueryAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetByIdsForWishlistAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);
    }
}

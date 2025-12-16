using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Interfaces.IRepositories.Catalog
{
    public interface IProductRepository
    {
        Task<bool> ExistsByExternalIdAsync(long externalId);
        Task AddAsync(Product product);
        Task AddRangeAsync(IEnumerable<Product> products);
        Task SaveChangesAsync();
    }
}

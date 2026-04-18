using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Interfaces.IRepositories.Catalog
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> IsExistAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

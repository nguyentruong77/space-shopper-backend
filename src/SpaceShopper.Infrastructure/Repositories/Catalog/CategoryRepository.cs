using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Infrastructure.Common;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Catalog
{
    public class CategoryRepository(SpaceShopperDbContext context) : Repository<Category>(context), ICategoryRepository
    {
        public async Task<bool> IsExistAsync(int id, CancellationToken cancellationToken = default)
        {
            return (await context.Categories.FirstOrDefaultAsync(e => e.IdClone == id)) is not null;
        }
        public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Categories.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Infrastructure.Persistence.Catalog;

namespace SpaceShopper.Infrastructure.Repositories.Catalog
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _context;
        public  ProductRepository(CatalogDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task AddRangeAsync(IEnumerable<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
        }

        public async Task<bool> ExistsByExternalIdAsync(long id)
        {
            return await _context.Products.AnyAsync(p => p.idClone == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

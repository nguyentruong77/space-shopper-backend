using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Shipping;
using SpaceShopper.Domain.Entities.Shipping;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Shipping
{
    public sealed class MethodShippingRepository(SpaceShopperDbContext context) : IMethodShippingRepository
    {
        private readonly SpaceShopperDbContext _context = context;

        public async Task AddEntityAsync(MethodShipping entity, CancellationToken cancellationToken = default)
        {
            await _context.MethodShippings.AddAsync(entity, cancellationToken);
        }

        public async Task<MethodShipping?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MethodShippings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<MethodShipping>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.MethodShippings
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }
    }
}

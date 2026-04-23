using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Promotions;
using SpaceShopper.Domain.Entities.Promotions;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Promotions
{
    public sealed class PromotionRepository(SpaceShopperDbContext context) : IPromotionRepository
    {
        private readonly SpaceShopperDbContext _context = context;

        public async Task<IReadOnlyList<Promotion>> GetActiveByCodesAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default)
        {
            if (codes.Count == 0)
            {
                return Array.Empty<Promotion>();
            }

            var normalizedCodes = codes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            if (normalizedCodes.Count == 0)
            {
                return Array.Empty<Promotion>();
            }

            return await _context.Promotions
                .AsNoTracking()
                .Where(p => p.IsActive && normalizedCodes.Contains(p.Code.ToUpper()))
                .ToListAsync(cancellationToken);
        }
    }
}

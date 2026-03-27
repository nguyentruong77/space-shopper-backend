using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Domain.Entities.Users;
using SpaceShopper.Infrastructure.Common;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Users
{
    public class UserRepository(SpaceShopperDbContext context) : Repository<User>(context), IUserRepository
    {
        private readonly SpaceShopperDbContext _dbContext = context;

        public async Task<User?> GetByIdWithTokensAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.UserTokens)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByIdWithAddressesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByIdWithPaymentMethodsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.UserPaymentMethods)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByIdWithProfileAggregateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.UserAddresses)
                .Include(u => u.UserPaymentMethods)
                .Include(u => u.UserTokens)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.UserTokens)
                .FirstOrDefaultAsync(
                    u => u.UserTokens.Any(t => t.RefreshToken == refreshToken),
                    cancellationToken);
        }
    }
}


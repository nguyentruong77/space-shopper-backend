using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Interfaces.IRepositories.Users
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithTokensAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithAddressesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithPaymentMethodsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithProfileAggregateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        Task<User?> GetByIdWithWishlistAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
    }
}


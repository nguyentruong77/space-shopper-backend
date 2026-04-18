using SpaceShopper.Application.Dtos.Users;

namespace SpaceShopper.Application.Interfaces.Iservices.Users
{
    public interface IWishlistService
    {
        Task<IReadOnlyList<WishlistItemDto>> GetWishlistAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<WishlistItemDto> AddWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task RemoveWishlistAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    }
}

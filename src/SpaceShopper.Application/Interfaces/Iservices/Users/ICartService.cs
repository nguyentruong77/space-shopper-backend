using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.Application.Interfaces.Iservices.Users
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<CartDto> UpdateCartQuantityAsync(Guid userId, Guid productId, UpdateCartQuantityRequest request, CancellationToken cancellationToken = default);
        Task RemoveCartItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    }
}

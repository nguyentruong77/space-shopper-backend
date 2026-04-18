using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.Application.Interfaces.Iservices.Users
{
    public interface IAddressService
    {
        Task<IReadOnlyList<UserAddressDto>> GetAddressesAsync(Guid userId, bool? isDefault, CancellationToken cancellationToken = default);
        Task<UserAddressDto> GetAddressByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
        Task<UserAddressDto> AddAddressAsync(Guid userId, AddAddressRequest request, CancellationToken cancellationToken = default);
        Task<UserAddressDto> EditAddressAsync(Guid userId, Guid addressId, EditAddressRequest request, CancellationToken cancellationToken = default);
        Task RemoveAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
    }
}

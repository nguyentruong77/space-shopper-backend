using SpaceShopper.Application.Dtos.Auth;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.Application.Interfaces.Iservices.Users
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task ResendEmailAsync(ResendEmailRequest request, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
        Task<AuthTokenResponse> ChangePasswordByCodeAsync(ChangePasswordByCodeRequest request, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
        Task<UserInfoDto> GetInfoAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserInfoDto> UpdateInfoAsync(Guid userId, UpdateUserInfoRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserAddressDto>> GetAddressesAsync(Guid userId, bool? isDefault, CancellationToken cancellationToken = default);
        Task<UserAddressDto> GetAddressByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
        Task<UserAddressDto> AddAddressAsync(Guid userId, AddAddressRequest request, CancellationToken cancellationToken = default);
        Task<UserAddressDto> EditAddressAsync(Guid userId, Guid addressId, EditAddressRequest request, CancellationToken cancellationToken = default);
        Task RemoveAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserPaymentDto>> GetPaymentsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserPaymentDto> GetPaymentByIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken = default);
        Task<UserPaymentDto> AddPaymentAsync(Guid userId, AddPaymentRequest request, CancellationToken cancellationToken = default);
        Task<UserPaymentDto> EditPaymentAsync(Guid userId, Guid paymentId, EditPaymentRequest request, CancellationToken cancellationToken = default);
        Task RemovePaymentAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken = default);
    }
}

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
    }
}

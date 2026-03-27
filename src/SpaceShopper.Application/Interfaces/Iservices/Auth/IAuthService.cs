using SpaceShopper.Application.Dtos.Auth;
using SpaceShopper.Application.Requests.Auth;

namespace SpaceShopper.Application.Interfaces.Iservices.Auth
{
    public interface IAuthService
    {
        Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<AuthTokenResponse> LoginByCodeAsync(LoginByCodeRequest request, CancellationToken cancellationToken = default);
        Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}


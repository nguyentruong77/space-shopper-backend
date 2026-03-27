using SpaceShopper.Domain.Entities.Users;
using System.Security.Claims;

namespace SpaceShopper.Application.Interfaces.Security
{
    public interface ITokenService
    {
        string GenerateAccessToken(User userDto);
        Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string token, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
        ClaimsPrincipal GetClaimsPrincipal(string token);
    }
}


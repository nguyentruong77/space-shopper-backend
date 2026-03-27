using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.Security;
using SpaceShopper.Domain.Entities.Users;
using SpaceShopper.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SpaceShopper.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _setting;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly SigningCredentials _credentials;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly ICacheService _cache;
        private readonly SHA512 _sha512;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<JwtOptions> options, ICacheService cache, ILogger<TokenService> logger)
        {
            _setting = options.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SecretKey));
            _credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512Signature);
            _tokenHandler = new JwtSecurityTokenHandler();
            _cache = cache;
            _sha512 = SHA512.Create();
            _logger = logger;
        }

        public string GenerateAccessToken(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var now = DateTime.UtcNow;
            var expires = now.Add(_setting.AccessTokenExpiration);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.MobilePhone, user.Phone),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("Username", user.Username)
        };

            var token = new JwtSecurityToken(
                issuer: _setting.Issuer,
                audience: _setting.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _credentials
            );

            return _tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var pepper = RandomNumberGenerator.GetBytes(64);
                var hash = _sha512.ComputeHash(pepper);

                var refreshToken = new RefreshToken
                {
                    UserId = userId,
                    Token = Convert.ToBase64String(hash)
                };

                var cacheKey = CacheKeys.ForRefreshToken(userId);
                await _cache.SetAsync(cacheKey, refreshToken, _setting.RefreshTokenAbsoluteExpiration, _setting.RefreshTokenSlidingExpiration, cancellationToken);

                return refreshToken.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating refresh token for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string token, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.ForRefreshToken(userId);
                var (Success, Value) = await _cache.TryGetValueAsync<RefreshToken>(cacheKey, cancellationToken);

                if (!Success || Value is null)
                {
                    return false;
                }

                return token == Value.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while validating refresh token for user {UserId}", userId);
                throw;
            }
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.ForRefreshToken(userId);
                await _cache.RemoveAsync(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token for user {UserId}", userId);
                throw;
            }
        }

        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _setting.Issuer,
                ValidAudience = _setting.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCulture))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Auth;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Auth;
using SpaceShopper.Application.Interfaces.Security;
using SpaceShopper.Application.Requests.Auth;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Services.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICacheService _cacheService;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITokenService jwtService,
            IPasswordHasher passwordHasher,
            ICacheService cacheService,
            IOptions<JwtOptions> jwtOptions,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _cacheService = cacheService;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new ValidationException(
                        ErrorCodes.Application.Validation,
                        ErrorMessages.Common.ValidationFailed,
                        new Dictionary<string, string[]>
                        {
                            ["email"] = string.IsNullOrWhiteSpace(request.Email) ? ["Email is required."] : [],
                            ["password"] = string.IsNullOrWhiteSpace(request.Password) ? ["Password is required."] : []
                        });
                }

                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (user is null)
                {
                    _logger.LogWarning("Login failed. User not found for email {Email}", request.Email);
                    throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
                }

                if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed. Invalid credentials for email {Email}", request.Email);
                    throw new ValidationException(ErrorCodes.Auth.InvalidCredentials, ErrorMessages.Auth.InvalidCredentials);
                }

                var now = DateTime.UtcNow;
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
                var refreshExpiresAt = now.Add(_jwtOptions.RefreshTokenAbsoluteExpiration);

                user.AddToken(refreshToken, refreshExpiresAt);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Login succeeded for user {UserId} with email {Email}", user.Id, request.Email);

                return new AuthTokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiresAt = now.Add(_jwtOptions.AccessTokenExpiration),
                    RefreshTokenExpiresAt = refreshExpiresAt
                };
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for email {Email}", request.Email);
                throw;
            }
        }

        public async Task<AuthTokenResponse> LoginByCodeAsync(LoginByCodeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                {
                    throw new ValidationException(
                        ErrorCodes.Application.Validation,
                        ErrorMessages.Common.ValidationFailed,
                        new Dictionary<string, string[]>
                        {
                            ["code"] = ["Code is required."]
                        });
                }

                var cacheKey = CacheKeys.AuthRegisterCode(request.Code);
                var pending = await _cacheService.GetAsync<PendingUserRegister>(cacheKey, cancellationToken);
                if (pending is null)
                {
                    _logger.LogWarning("Login-by-code failed. Invalid or expired code {Code}", request.Code);
                    throw new ValidationException(ErrorCodes.Auth.InvalidLoginCode, ErrorMessages.Auth.InvalidLoginCode);
                }

                var user = await _userRepository.GetByEmailAsync(pending.Email, cancellationToken);
                if (user is null)
                {
                    user = new User
                    {
                        Username = pending.Email,
                        Name = pending.Name,
                        PasswordHash = pending.PasswordHash,
                        Phone = string.Empty,
                        Role = Domain.Enums.UserRole.User
                    };

                    await _userRepository.AddEntityAsync(user, cancellationToken);
                }

                await _cacheService.RemoveAsync(cacheKey, cancellationToken);

                var now = DateTime.UtcNow;
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
                var refreshExpiresAt = now.Add(_jwtOptions.RefreshTokenAbsoluteExpiration);

                user.AddToken(refreshToken, refreshExpiresAt);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Login-by-code succeeded for email {Email}", pending.Email);

                return new AuthTokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiresAt = now.Add(_jwtOptions.AccessTokenExpiration),
                    RefreshTokenExpiresAt = refreshExpiresAt
                };
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login-by-code for code {Code}", request.Code);
                throw;
            }
        }

        public async Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    throw new ValidationException(
                        ErrorCodes.Application.Validation,
                        ErrorMessages.Common.ValidationFailed,
                        new Dictionary<string, string[]>
                        {
                            ["refreshToken"] = ["Refresh token is required."]
                        });
                }

                var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
                if (user is null)
                {
                    _logger.LogWarning("Refresh token invalid. Token not found");
                    throw new UnauthorizedException(ErrorCodes.Auth.RefreshTokenInvalid, ErrorMessages.Auth.RefreshTokenInvalid);
                }

                var now = DateTime.UtcNow;
                var validToken = user.FindValidToken(request.RefreshToken, now);
                var isValidInCache = await _jwtService.ValidateRefreshTokenAsync(user.Id, request.RefreshToken, cancellationToken);

                if (!isValidInCache && validToken is null)
                {
                    _logger.LogWarning("Refresh token invalid in both Redis and DB for user {UserId}", user.Id);
                    throw new UnauthorizedException(ErrorCodes.Auth.RefreshTokenInvalid, ErrorMessages.Auth.RefreshTokenInvalid);
                }

                if (!isValidInCache && validToken is not null)
                {
                    _logger.LogInformation("Refresh token validated by DB fallback for user {UserId}", user.Id);
                }

                if (isValidInCache && validToken is null)
                {
                    _logger.LogError("Token consistency issue: token exists in Redis but not valid in DB for user {UserId}", user.Id);
                    throw new UnauthorizedException(ErrorCodes.Auth.RefreshTokenInvalid, ErrorMessages.Auth.RefreshTokenInvalid);
                }

                var accessToken = _jwtService.GenerateAccessToken(user);
                await _jwtService.RevokeRefreshTokenAsync(user.Id, cancellationToken);
                var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
                var newRefreshExpiresAt = now.Add(_jwtOptions.RefreshTokenAbsoluteExpiration);

                user.RevokeToken(validToken!);
                user.AddToken(newRefreshToken, newRefreshExpiresAt);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Refresh token succeeded for user {UserId}", user.Id);

                return new AuthTokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiresAt = now.Add(_jwtOptions.AccessTokenExpiration),
                    RefreshTokenExpiresAt = newRefreshExpiresAt
                };
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during refresh-token flow");
                throw;
            }
        }
    }
}


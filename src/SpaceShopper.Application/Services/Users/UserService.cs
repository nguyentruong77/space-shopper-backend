using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using SpaceShopper.Application.Common.Email;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Auth;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Common;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Interfaces.Security;
using SpaceShopper.Application.Requests.Users;
using SpaceShopper.Domain.Entities.Users;
using SpaceShopper.Domain.Enums;

namespace SpaceShopper.Application.Services.Users
{
    public sealed class UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ICacheService cacheService,
        IEmailService emailService,
        IMapper mapper,
        IOptions<JwtOptions> jwtOptions,
        ILogger<UserService> logger) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IEmailService _emailService = emailService;
        private readonly IMapper _mapper = mapper;
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly ILogger<UserService> _logger = logger;

        public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var email = ResolveEmail(request.Email, request.UsernameAlias);
            ValidateEmail(email);
            ValidatePasswordPolicy(request.Password);

            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser is not null)
            {
                throw new DomainException(ErrorCodes.User.EmailAlreadyExists, ErrorMessages.User.EmailAlreadyExists);
            }

            var code = Guid.NewGuid().ToString("N");
            var pending = new PendingUserRegister
            {
                Email = email,
                Name = request.Name,
                PasswordHash = _passwordHasher.Hash(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _cacheService.SetAsync(RegisterCodeCacheKey(code), pending, TimeSpan.FromMinutes(15), cancellationToken: cancellationToken);
            await _emailService.SendAsync(
                email,
                "Verify your account",
                $"Your verification code: {code}",
                EmailTemplateKind.Verification,
                cancellationToken);
            _logger.LogInformation("Register pending created for email {Email}", email);
        }

        public async Task ResendEmailAsync(ResendEmailRequest request, CancellationToken cancellationToken = default)
        {
            var email = ResolveEmail(request.Email, request.UsernameAlias);
            ValidateEmail(email);

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
            }

            var code = Guid.NewGuid().ToString("N");
            var pending = new PendingUserRegister
            {
                Email = email,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                CreatedAt = DateTime.UtcNow
            };

            await _cacheService.SetAsync(RegisterCodeCacheKey(code), pending, TimeSpan.FromMinutes(15), cancellationToken: cancellationToken);
            await _emailService.SendAsync(
                email,
                "Resend verification",
                $"Your verification code: {code}",
                EmailTemplateKind.ResendVerification,
                cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var email = ResolveEmail(request.Email, request.UsernameAlias);
            ValidateEmail(email);

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
            }

            var code = Guid.NewGuid().ToString("N");
            await _cacheService.SetAsync(ResetCodeCacheKey(code), user.Id, TimeSpan.FromMinutes(15), cancellationToken: cancellationToken);
            await _emailService.SendAsync(
                email,
                "Reset password",
                $"Your reset code: {code}",
                EmailTemplateKind.PasswordReset,
                cancellationToken);
        }

        public async Task<AuthTokenResponse> ChangePasswordByCodeAsync(ChangePasswordByCodeRequest request, CancellationToken cancellationToken = default)
        {
            ValidatePasswordPolicy(request.Password);
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                throw new ValidationException(ErrorCodes.Application.Validation, ErrorMessages.Common.ValidationFailed);
            }

            var userId = await _cacheService.GetAndRemoveAsync<Guid>(ResetCodeCacheKey(request.Code), cancellationToken);
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedException(ErrorCodes.User.CodeInvalidOrExpired, ErrorMessages.User.CodeInvalidOrExpired);
            }

            var user = await _userRepository.GetByIdWithTokensAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            user.ChangePassword(_passwordHasher.Hash(request.Password));
            user.IncrementTokenVersion();

            var now = DateTime.UtcNow;
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
            var refreshExpiresAt = now.Add(_jwtOptions.RefreshTokenAbsoluteExpiration);
            user.AddToken(refreshToken, refreshExpiresAt);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = now.Add(_jwtOptions.AccessTokenExpiration),
                RefreshTokenExpiresAt = refreshExpiresAt
            };
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            ValidatePasswordPolicy(request.NewPassword);
            var user = await _userRepository.GetByIdWithTokensAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedException(ErrorCodes.User.PasswordIncorrect, ErrorMessages.User.PasswordIncorrect);
            }

            user.ChangePassword(_passwordHasher.Hash(request.NewPassword));
            user.IncrementTokenVersion();
            await _tokenService.RevokeRefreshTokenAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserInfoDto> GetInfoAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
            return _mapper.Map<UserInfoDto>(user);
        }

        public async Task<UserInfoDto> UpdateInfoAsync(Guid userId, UpdateUserInfoRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
            user.UpdateProfile(request.Name, request.Phone, request.Avatar, request.Fb, request.BirthDay, request.Gender);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserInfoDto>(user);
        }

        public async Task<IReadOnlyList<UserAddressDto>> GetAddressesAsync(Guid userId, bool? isDefault, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var source = isDefault.HasValue ? user.UserAddresses.Where(x => x.Default == isDefault.Value) : user.UserAddresses;
            return source.Select(x => _mapper.Map<UserAddressDto>(x)).ToList();
        }

        public async Task<UserAddressDto> GetAddressByIdAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var address = user.UserAddresses.FirstOrDefault(x => x.Id == addressId)
                ?? throw new NotFoundException(ErrorCodes.User.ChildEntityNotFound, ErrorMessages.User.ChildEntityNotFound);
            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task<UserAddressDto> AddAddressAsync(Guid userId, AddAddressRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var address = new UserAddress
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Province = request.Province,
                District = request.District,
                Address = request.Address
            };

            var created = user.AddAddress(address, request.IsDefault);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserAddressDto>(created);
        }

        public async Task<UserAddressDto> EditAddressAsync(Guid userId, Guid addressId, EditAddressRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                var updated = user.UpdateAddress(addressId, request.FullName, request.Email, request.Phone, request.Province, request.District, request.Address, request.IsDefault);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return _mapper.Map<UserAddressDto>(updated);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException(ErrorCodes.User.ChildEntityNotFound, ErrorMessages.User.ChildEntityNotFound);
            }
        }

        public async Task RemoveAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                user.RemoveAddress(addressId);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException(ErrorCodes.User.ChildEntityNotFound, ErrorMessages.User.ChildEntityNotFound);
            }
            catch (InvalidOperationException)
            {
                throw new DomainException(ErrorCodes.User.DefaultEntityDeleteForbidden, ErrorMessages.User.DefaultEntityDeleteForbidden);
            }
        }

        public async Task<IReadOnlyList<UserPaymentDto>> GetPaymentsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithPaymentMethodsAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);
            return user.UserPaymentMethods.Select(x => _mapper.Map<UserPaymentDto>(x)).ToList();
        }

        public async Task<UserPaymentDto> GetPaymentByIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithPaymentMethodsAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var payment = user.UserPaymentMethods.FirstOrDefault(x => x.Id == paymentId)
                ?? throw new NotFoundException(ErrorCodes.User.ChildEntityNotFound, ErrorMessages.User.ChildEntityNotFound);
            return _mapper.Map<UserPaymentDto>(payment);
        }

        public async Task<UserPaymentDto> AddPaymentAsync(Guid userId, AddPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithPaymentMethodsAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            var payment = new UserPaymentMethod
            {
                CardName = request.CardName,
                CardNumber = request.CardNumber,
                Cvv = request.Cvv,
                ExpirationDate = request.Expired,
                Type = request.Type
            };

            var created = user.AddPaymentMethod(payment, request.IsDefault);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserPaymentDto>(created);
        }

        public async Task<UserPaymentDto> EditPaymentAsync(Guid userId, Guid paymentId, EditPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithPaymentMethodsAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                var updated = user.UpdatePaymentMethod(paymentId, request.CardName, request.CardNumber, request.Expired, request.Type, request.IsDefault);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return _mapper.Map<UserPaymentDto>(updated);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException(ErrorCodes.User.ChildEntityNotFound, ErrorMessages.User.ChildEntityNotFound);
            }
        }

        public async Task RemovePaymentAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithPaymentMethodsAsync(userId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.User.UserNotFound, ErrorMessages.User.UserNotFound);

            try
            {
                user.RemovePaymentMethod(paymentId);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException(ErrorCodes.User.ChildEntityNotFound, ErrorMessages.User.ChildEntityNotFound);
            }
            catch (InvalidOperationException)
            {
                throw new DomainException(ErrorCodes.User.DefaultEntityDeleteForbidden, ErrorMessages.User.DefaultEntityDeleteForbidden);
            }
        }

        private static string ResolveEmail(string? email, string? usernameAlias)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(usernameAlias))
            {
                return usernameAlias.Trim();
            }

            throw new ValidationException(ErrorCodes.Application.Validation, ErrorMessages.Common.ValidationFailed);
        }

        private static void ValidateEmail(string email)
        {
            if (!email.Contains('@'))
            {
                throw new ValidationException(ErrorCodes.User.EmailInvalid, ErrorMessages.User.EmailInvalid);
            }
        }

        private static void ValidatePasswordPolicy(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ValidationException(ErrorCodes.User.PasswordPolicyViolated, ErrorMessages.User.PasswordPolicyViolated);
            }
        }

        private static string RegisterCodeCacheKey(string code) => $"user:register-code:{code}";
        private static string ResetCodeCacheKey(string code) => $"user:reset-code:{code}";
    }
}

using AutoMapper;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Requests.Users;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Services.Users
{
    public sealed class UserPaymentMethodService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IUserPaymentMethodService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

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
    }
}

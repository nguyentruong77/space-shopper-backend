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
    public sealed class AddressService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IAddressService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

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
    }
}

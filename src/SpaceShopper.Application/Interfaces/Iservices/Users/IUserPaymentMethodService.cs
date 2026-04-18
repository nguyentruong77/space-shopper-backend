using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.Application.Interfaces.Iservices.Users
{
    public interface IUserPaymentMethodService
    {
        Task<IReadOnlyList<UserPaymentDto>> GetPaymentsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserPaymentDto> GetPaymentByIdAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken = default);
        Task<UserPaymentDto> AddPaymentAsync(Guid userId, AddPaymentRequest request, CancellationToken cancellationToken = default);
        Task<UserPaymentDto> EditPaymentAsync(Guid userId, Guid paymentId, EditPaymentRequest request, CancellationToken cancellationToken = default);
        Task RemovePaymentAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken = default);
    }
}

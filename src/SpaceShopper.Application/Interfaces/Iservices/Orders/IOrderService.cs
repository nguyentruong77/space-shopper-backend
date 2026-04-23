using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Application.Requests.Orders;

namespace SpaceShopper.Application.Interfaces.Iservices.Orders
{
    public interface IOrderService
    {
        Task<PreCheckoutDto> PreCheckoutAsync(Guid userId, PreCheckoutRequest request, CancellationToken cancellationToken = default);
        Task<CheckoutResultDto> CheckoutAsync(Guid userId, CheckoutRequest request, CancellationToken cancellationToken = default);
    }
}

using SpaceShopper.Application.Dtos.Shipping;

namespace SpaceShopper.Application.Interfaces.Iservices.Shipping
{
    public interface IShippingMethodService
    {
        Task<IReadOnlyList<ShippingMethodDto>> GetShippingMethodsAsync(CancellationToken cancellationToken = default);
    }
}

using AutoMapper;
using SpaceShopper.Application.Dtos.Shipping;
using SpaceShopper.Application.Interfaces.IRepositories.Shipping;
using SpaceShopper.Application.Interfaces.Iservices.Shipping;

namespace SpaceShopper.Application.Services.Shipping
{
    public sealed class ShippingMethodService(
        IMethodShippingRepository methodShippingRepository,
        IMapper mapper) : IShippingMethodService
    {
        private readonly IMethodShippingRepository _methodShippingRepository = methodShippingRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IReadOnlyList<ShippingMethodDto>> GetShippingMethodsAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _methodShippingRepository.GetActiveAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<ShippingMethodDto>>(entities);
        }
    }
}

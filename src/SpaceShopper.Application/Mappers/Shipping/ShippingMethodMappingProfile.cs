using AutoMapper;
using SpaceShopper.Application.Dtos.Shipping;
using SpaceShopper.Domain.Entities.Shipping;

namespace SpaceShopper.Application.Mappers.Shipping
{
    public sealed class ShippingMethodMappingProfile : Profile
    {
        public ShippingMethodMappingProfile()
        {
            CreateMap<MethodShipping, ShippingMethodDto>();
        }
    }
}

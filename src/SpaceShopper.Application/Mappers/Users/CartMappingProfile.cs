using AutoMapper;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Mappers.Users
{
    public sealed class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<(UserCart CartLine, Product Product), CartItemDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.CartLine.ProductId))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.CartLine.Quantity))
                .ForMember(d => d.Product, opt => opt.MapFrom(s => s.Product));
        }
    }
}

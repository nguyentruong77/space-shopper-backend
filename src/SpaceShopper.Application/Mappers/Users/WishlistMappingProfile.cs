using AutoMapper;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Mappers.Users
{
    public sealed class WishlistMappingProfile : Profile
    {
        public WishlistMappingProfile()
        {
            CreateMap<(WishlistItem Item, Product Product), WishlistItemDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.Item.ProductId))
                .ForMember(d => d.Product, opt => opt.MapFrom(s => s.Product));
        }
    }
}

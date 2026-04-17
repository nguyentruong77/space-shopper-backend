using AutoMapper;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Mappers.Catalog
{
    public sealed class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductListItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdClone));
        }
    }
}

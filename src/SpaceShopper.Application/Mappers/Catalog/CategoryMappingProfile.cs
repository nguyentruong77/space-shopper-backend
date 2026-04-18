using AutoMapper;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Mappers.Catalog
{
    public sealed class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryDto>();
        }
    }
}

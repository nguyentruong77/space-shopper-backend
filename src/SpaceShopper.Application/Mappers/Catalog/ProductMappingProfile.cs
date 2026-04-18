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

            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.ProductStock))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ProductReviews));

            CreateMap<ProductImage, ProductImageDetailDto>();
            CreateMap<ProductStock, ProductStockDetailDto>();
            CreateMap<ProductReview, ProductReviewDetailDto>();
        }
    }
}

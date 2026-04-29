using AutoMapper;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Domain.Entities.Catalog;

namespace SpaceShopper.Application.Mappers.Catalog
{
    public sealed class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.NameUser))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvtUrl))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Star))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn));
        }
    }
}

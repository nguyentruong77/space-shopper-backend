using AutoMapper;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Application.Mappers.Users
{
    public sealed class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserInfoDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Username));

            CreateMap<UserAddress, UserAddressDto>()
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.Default));

            CreateMap<UserPaymentMethod, UserPaymentDto>()
                .ForMember(dest => dest.Expired, opt => opt.MapFrom(src => src.ExpirationDate))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.Default))
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)));
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 8)
            {
                return "****";
            }

            return $"{cardNumber[..4]}****{cardNumber[^4..]}";
        }
    }
}

using AutoMapper;
using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Domain.Entities.Orders;

namespace SpaceShopper.Application.Mappers.Orders
{
    public sealed class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, CheckoutResultDto>()
                .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ItemsSubtotal, opt => opt.MapFrom(s => s.SubTotal))
                .ForMember(d => d.ShippingFee, opt => opt.MapFrom(s => s.OrderShipping.ShippingPrice))
                .ForMember(d => d.GrandTotal, opt => opt.MapFrom(s => s.Total))
                .ForMember(d => d.OrderStatus, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.AppliedOrderPromotionCode, opt => opt.MapFrom(s =>
                    s.OrderPromotions.FirstOrDefault(p => !p.IsShippingDiscount) != null
                        ? s.OrderPromotions.First(p => !p.IsShippingDiscount).PromotionCode
                        : null))
                .ForMember(d => d.AppliedShippingPromotionCode, opt => opt.MapFrom(s =>
                    s.OrderPromotions.FirstOrDefault(p => p.IsShippingDiscount) != null
                        ? s.OrderPromotions.First(p => p.IsShippingDiscount).PromotionCode
                        : null));
        }
    }
}

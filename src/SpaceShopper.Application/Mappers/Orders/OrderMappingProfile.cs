using AutoMapper;
using SpaceShopper.Application.Dtos.Orders;
using SpaceShopper.Domain.Entities.Orders;

namespace SpaceShopper.Application.Mappers.Orders
{
    public sealed class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderShipping, OrderShippingDto>()
                .ForMember(d => d.ShippingMethod, opt => opt.MapFrom(s => s.ShippingMethodCode));

            CreateMap<OrderDetail, OrderListItemDto>();

            CreateMap<Order, OrderListDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString().ToLowerInvariant()))
                .ForMember(d => d.Shipping, opt => opt.MapFrom(s => s.OrderShipping))
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.OrderDetails));

            CreateMap<Order, OrderDetailDto>()
                .IncludeBase<Order, OrderListDto>()
                .ForMember(d => d.AppliedOrderPromotionCode, opt => opt.MapFrom(s =>
                    s.OrderPromotions
                        .Where(p => !p.IsShippingDiscount)
                        .Select(p => p.PromotionCode)
                        .FirstOrDefault()))
                .ForMember(d => d.AppliedShippingPromotionCode, opt => opt.MapFrom(s =>
                    s.OrderPromotions
                        .Where(p => p.IsShippingDiscount)
                        .Select(p => p.PromotionCode)
                        .FirstOrDefault()));

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

using SpaceShopper.Domain.Common;
using SpaceShopper.Domain.Enums;

namespace SpaceShopper.Domain.Entities.Orders
{
    public class Order : SoftDeletableAggregateRoot
    {
        public Guid UserId { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public int TotalQuantity { get; set; }
        public decimal ViewCartTotal { get; set; }
        public string Type { get; set; }
        public OrderStatus Status { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
        public string OrderCode { get; set; } = string.Empty;
        public decimal ProductDiscountAmount { get; set; }
        public decimal ShippingDiscountAmount { get; set; }
        public string? Note { get; set; }
        public string? PaymentMethod { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<OrderPromotion> OrderPromotions { get; set; } = new List<OrderPromotion>();
        public OrderShipping OrderShipping { get; set; }
    }
}

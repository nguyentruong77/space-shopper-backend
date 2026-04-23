using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Orders
{
    public class OrderPromotion : BaseEntity
    {
        public Guid OrderId { get; set; }
        public string PromotionCode { get; set; } = string.Empty;
        public bool IsShippingDiscount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public Order Order { get; set; } = null!;
    }
}

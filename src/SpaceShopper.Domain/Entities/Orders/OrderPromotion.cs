using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShopper.Domain.Entities.Orders
{
    public class OrderPromotion
    {
        public Guid OrderId { get; set; }
        public string  Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }
    }
}

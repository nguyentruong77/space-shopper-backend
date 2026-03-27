using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShopper.Domain.Entities.Orders
{
    public class OrderShipping
    {
        public Guid OrderId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string ShippingMethod { get; set; }
        public decimal ShippingPrice { get; set; }
        public string Address { get; set; }
    }
}

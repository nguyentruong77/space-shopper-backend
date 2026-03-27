using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Orders
{
    public class Promotion : BaseEntity
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Type { get; set; }
    }
}

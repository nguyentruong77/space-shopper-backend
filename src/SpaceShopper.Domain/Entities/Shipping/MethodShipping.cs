using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Shipping
{
    public class MethodShipping : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? EstimatedDaysMin { get; set; }
        public int? EstimatedDaysMax { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

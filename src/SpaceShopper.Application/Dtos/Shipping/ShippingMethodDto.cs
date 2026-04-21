namespace SpaceShopper.Application.Dtos.Shipping
{
    public sealed class ShippingMethodDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public int? EstimatedDaysMin { get; init; }
        public int? EstimatedDaysMax { get; init; }
        public bool IsActive { get; init; }
    }
}

namespace SpaceShopper.Application.Dtos.Catalog
{
    public sealed class ProductListItemDto
    {
        public long Id { get; init; }
        public Guid CategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public decimal RealPrice { get; init; }
        public decimal DiscountRate { get; init; }
        public decimal RatingAverage { get; init; }
        public int ReviewCount { get; init; }
        public string? ShortDescription { get; init; }
        public string? ThumbnailUrl { get; init; }
    }
}

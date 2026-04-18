namespace SpaceShopper.Application.Dtos.Catalog
{
    public sealed class ProductDetailDto
    {
        public Guid Id { get; init; }
        public long IdClone { get; init; }
        public Guid CategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
        public string? ShortDescription { get; init; }
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public decimal RealPrice { get; init; }
        public decimal DiscountRate { get; init; }
        public decimal RatingAverage { get; init; }
        public int ReviewCount { get; init; }
        public string? ThumbnailUrl { get; init; }
        public IReadOnlyList<ProductImageDetailDto> Images { get; init; } = [];
        public ProductStockDetailDto? Stock { get; init; }
        public IReadOnlyList<ProductReviewDetailDto> Reviews { get; init; } = [];
    }

    public sealed class ProductImageDetailDto
    {
        public Guid Id { get; init; }
        public string? Label { get; init; }
        public int Position { get; init; }
        public string? BaseUrl { get; init; }
        public string? ThumbnailUrl { get; init; }
        public string? SmallUrl { get; init; }
        public string? MediumUrl { get; init; }
        public string? LargeUrl { get; init; }
        public bool IsGallery { get; init; }
    }

    public sealed class ProductStockDetailDto
    {
        public int Quantity { get; init; }
        public int MinSaleQty { get; init; }
        public int MaxSaleQty { get; init; }
        public bool PreOrder { get; init; }
    }

    public sealed class ProductReviewDetailDto
    {
        public Guid Id { get; init; }
        public Guid? OrderId { get; init; }
        public Guid? UserId { get; init; }
        public DateTime CreatedOn { get; init; }
        public string NameUser { get; init; } = string.Empty;
        public string? AvtUrl { get; init; }
        public string Content { get; init; } = string.Empty;
        public int Star { get; init; }
    }
}

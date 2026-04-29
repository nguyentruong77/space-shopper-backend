using SpaceShopper.Application.Dtos.Common;

namespace SpaceShopper.Application.Dtos.Catalog
{
    public sealed class ProductReviewDto
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid? UserId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public sealed class ReviewSummaryDto
    {
        public decimal AverageRating { get; init; }
        public int TotalReviews { get; init; }
    }

    public sealed class ProductReviewListDto
    {
        public PagedResult<ProductReviewDto> Reviews { get; init; } = new();
        public ReviewSummaryDto Summary { get; init; } = new();
    }

    public sealed class AddProductReviewResultDto
    {
        public ProductReviewDto Review { get; init; } = new();
        public ReviewSummaryDto Summary { get; init; } = new();
    }
}

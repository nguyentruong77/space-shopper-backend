using SpaceShopper.Domain.Common;
using System.Linq;

namespace SpaceShopper.Domain.Entities.Catalog
{
    public class Product : SoftDeletableAggregateRoot
    {
        public Guid CategoryId { get; set; }
        public long IdClone { get; set; }
        public string Name { get; set; }
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal RealPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal RatingAverage { get; set; }
        public int ReviewCount { get; set; }
        public string? ThumbnailUrl { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ProductStock ProductStock { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public Category Category { get; set; }

        public ProductReview AddReview(Guid userId, string fullName, string? avatarUrl, int rating, string comment, Guid? orderId = null)
        {
            if (rating is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be in range 1..5.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Comment is required.", nameof(comment));
            }

            var review = new ProductReview
            {
                ProductId = Id,
                OrderId = orderId,
                UserId = userId,
                NameUser = fullName,
                AvtUrl = avatarUrl,
                Content = comment.Trim(),
                Star = rating,
                CreatedOn = DateTime.UtcNow
            };

            ProductReviews.Add(review);
            RecalculateRatingSummary();
            return review;
        }

        public void RecalculateRatingSummary()
        {
            var activeReviews = ProductReviews.Where(x => !x.IsDeleted).ToList();
            ReviewCount = activeReviews.Count;
            RatingAverage = ReviewCount == 0
                ? 0m
                : decimal.Round(activeReviews.Average(x => (decimal)x.Star), 2, MidpointRounding.AwayFromZero);
        }
    }
}

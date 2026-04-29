using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Orders
{
    public class OrderDetail
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int IdCloneProduct { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Price { get; set; }
        public decimal RealPrice { get; set; }
        public float RatingAverage { get; set; }
        public int ReviewCount { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Slug { get; set; }

        /// <summary>
        /// True when the buyer has submitted a product review for this order line (finished order + product).
        /// </summary>
        public bool IsReviewed { get; set; }
    }
}

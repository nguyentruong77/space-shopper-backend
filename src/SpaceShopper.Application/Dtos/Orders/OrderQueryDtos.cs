namespace SpaceShopper.Application.Dtos.Orders
{
    public sealed class OrderShippingDto
    {
        public string FullName { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Province { get; init; } = string.Empty;
        public string District { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string ShippingMethod { get; init; } = string.Empty;
        public decimal ShippingPrice { get; init; }
    }

    public sealed class OrderListItemDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal RealPrice { get; init; }
        public decimal TotalPrice { get; init; }
        public float RatingAverage { get; init; }
        public int ReviewCount { get; init; }
        public string? ThumbnailUrl { get; init; }
        public string? Slug { get; init; }
    }

    public class OrderListDto
    {
        public Guid Id { get; init; }
        public decimal Total { get; init; }
        public decimal SubTotal { get; init; }
        public decimal Tax { get; init; }
        public int TotalQuantity { get; init; }
        public decimal ViewCartTotal { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string PaymentStatus { get; init; } = string.Empty;
        public string OrderCode { get; init; } = string.Empty;
        public decimal ProductDiscountAmount { get; init; }
        public decimal ShippingDiscountAmount { get; init; }
        public string? Note { get; init; }
        public string? PaymentMethod { get; init; }
        public DateTime CreatedOn { get; init; }
        public OrderShippingDto? Shipping { get; init; }
        public IReadOnlyList<OrderListItemDto> Items { get; init; } = [];
    }

    public sealed class OrderDetailDto : OrderListDto
    {
        public string? AppliedOrderPromotionCode { get; init; }
        public string? AppliedShippingPromotionCode { get; init; }
    }

    public sealed class OrderStatusCountDto
    {
        public string Status { get; init; } = string.Empty;
        public int Count { get; init; }
    }
}

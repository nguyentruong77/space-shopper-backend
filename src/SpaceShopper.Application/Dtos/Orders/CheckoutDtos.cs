namespace SpaceShopper.Application.Dtos.Orders
{
    public class PreCheckoutDto
    {
        public decimal ItemsSubtotal { get; init; }
        public decimal ShippingFee { get; init; }
        public decimal ProductDiscountAmount { get; init; }
        public decimal ShippingDiscountAmount { get; init; }
        public decimal GrandTotal { get; init; }
        public string? AppliedOrderPromotionCode { get; init; }
        public string? AppliedShippingPromotionCode { get; init; }
        public string Currency { get; init; } = "VND";
    }

    public sealed class CheckoutResultDto : PreCheckoutDto
    {
        public Guid OrderId { get; init; }
        public string OrderCode { get; init; } = string.Empty;
        public string OrderStatus { get; init; } = string.Empty;
        public string PaymentStatus { get; init; } = string.Empty;
    }
}

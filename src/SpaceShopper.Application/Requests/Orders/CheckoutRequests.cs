using System.ComponentModel.DataAnnotations;

namespace SpaceShopper.Application.Requests.Orders
{
    public class PreCheckoutRequest
    {
        [Required]
        [MinLength(1)]
        public IReadOnlyList<Guid> ListItems { get; set; } = Array.Empty<Guid>();

        public string? OrderPromotionCode { get; set; }
        public string? ShippingPromotionCode { get; set; }
    }

    public sealed class CheckoutRequest : PreCheckoutRequest
    {
        [Required]
        public Guid? ShippingAddressId { get; set; }

        [Required]
        public string ShippingMethod { get; set; } = string.Empty;
    }
}

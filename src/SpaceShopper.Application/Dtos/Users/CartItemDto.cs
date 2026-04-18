using SpaceShopper.Application.Dtos.Catalog;

namespace SpaceShopper.Application.Dtos.Users
{
    public sealed class CartItemDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public ProductListItemDto Product { get; init; } = null!;
    }
}

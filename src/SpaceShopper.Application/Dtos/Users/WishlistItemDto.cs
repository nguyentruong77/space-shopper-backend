using SpaceShopper.Application.Dtos.Catalog;

namespace SpaceShopper.Application.Dtos.Users
{
    public sealed class WishlistItemDto
    {
        public Guid ProductId { get; init; }
        public ProductListItemDto Product { get; init; } = null!;
    }
}

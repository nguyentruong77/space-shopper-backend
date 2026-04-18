namespace SpaceShopper.Application.Dtos.Users
{
    public sealed class CartDto
    {
        public decimal SubTotal { get; init; }
        public int TotalQuantity { get; init; }
        public IReadOnlyList<CartItemDto> ListItems { get; init; } = Array.Empty<CartItemDto>();
    }
}

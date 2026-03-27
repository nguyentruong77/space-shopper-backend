namespace SpaceShopper.Domain.Entities.Users
{
    public class WishlistItem
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}

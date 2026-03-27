namespace SpaceShopper.Domain.Entities.Users
{
    public class UserCart
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

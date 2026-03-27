namespace SpaceShopper.Domain.Models
{
    public class RefreshToken
    {
        public Guid UserId { get; set; }
        public required string Token { get; set; }
    }
}

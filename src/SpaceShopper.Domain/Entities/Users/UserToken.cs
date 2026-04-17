using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Users
{
    public class UserToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public int TokenVersion { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        
        public bool IsExpired(DateTime now) => ExpiresAt <= now;
    }
}

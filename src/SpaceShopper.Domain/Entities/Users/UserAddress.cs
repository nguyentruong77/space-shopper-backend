using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Users
{
    public class UserAddress : BaseEntity
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public bool Default { get; set; }
    }
}

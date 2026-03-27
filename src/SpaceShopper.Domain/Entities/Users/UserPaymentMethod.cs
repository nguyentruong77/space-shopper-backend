using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Users
{
    public class UserPaymentMethod : BaseEntity
    {
        public Guid UserId { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Cvv { get; set; }
        public string Type { get; set; }
        public bool Default { get; set; }
    }
}

using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Catalog
{
    public class ProductReview : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string NameUser { get; set; }
        public string? AvtUrl { get; set; }
        public string Content { get; set; }
        public int Star { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}

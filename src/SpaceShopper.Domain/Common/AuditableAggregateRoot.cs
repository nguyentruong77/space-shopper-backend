namespace SpaceShopper.Domain.Common
{
    public abstract class AuditableAggregateRoot : AggregateRoot
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}

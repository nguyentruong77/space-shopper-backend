namespace SpaceShopper.Domain.Common
{
    public abstract class SoftDeletableAggregateRoot : AuditableAggregateRoot
    {
        public DateTime? DeletedOn { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}

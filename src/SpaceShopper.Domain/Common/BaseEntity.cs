namespace SpaceShopper.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public BaseEntity(Guid id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}

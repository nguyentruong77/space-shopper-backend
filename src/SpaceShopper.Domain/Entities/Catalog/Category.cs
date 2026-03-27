using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Catalog
{
    public class Category : SoftDeletableAggregateRoot
    {
        public string Title { get; set; }
        public int IdClone { get; set; }
        public Guid ParentId { get; set; }
        public string? Slug { get; set; }
        public int Status { get; set; }
        public int Position { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

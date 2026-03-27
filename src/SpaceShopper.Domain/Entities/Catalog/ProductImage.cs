using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Catalog
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string? Label { get; set; }
        public int Position { get; set; }
        public string? BaseUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? SmallUrl { get; set; }
        public string? MediumUrl { get; set; }
        public string? LargeUrl { get; set; }
        public bool IsGallery { get; set; }
    }
}

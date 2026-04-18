namespace SpaceShopper.Application.Dtos.Catalog
{
    public sealed class CategoryDto
    {
        public Guid Id { get; init; }
        public int IdClone { get; init; }
        public Guid ParentId { get; init; }
        public int Position { get; init; }
        public int Status { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Slug { get; init; }
    }
}

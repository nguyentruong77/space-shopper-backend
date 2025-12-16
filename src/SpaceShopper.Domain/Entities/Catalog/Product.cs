using SpaceShopper.Domain.ValueObjects;

namespace SpaceShopper.Domain.Entities.Catalog
{
    public class Product
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public long idClone { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
        public Money Price { get; set; }
        public Money? RealPrice { get; set; }
        public decimal? PriceUsd { get; set; }
        public decimal RatingAverage { get; set; }
        public int ReviewCount { get; set; }
        private Product() {}

        public static Product Create(long idClone, string name, Money price, Money realPrice, string description, string slug)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            return new Product
            {
                Name = name,
                Description = description,
                Price = price,
                RealPrice = realPrice,
                idClone = idClone,
                Slug = slug
            };
        }
    }
}

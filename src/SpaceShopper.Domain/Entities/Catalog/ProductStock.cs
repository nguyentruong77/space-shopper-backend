namespace SpaceShopper.Domain.Entities.Catalog
{
    public class ProductStock
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int MinSaleQty { get; set; }
        public int MaxSaleQty { get; set; }
        public bool PreOrder { get; set; }
    }
}

using SpaceShopper.Application.Requests.Common;
using System.Globalization;

namespace SpaceShopper.Application.Requests.Catalog
{
    public class ProductSearchRequest : PageRequest
    {
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? FilterRating { get; set; }
        public ProductSort? Sort { get; set; }

        public override string ToString()
        {
            var keyword = string.IsNullOrWhiteSpace(Keyword) ? "" : Keyword.Trim().ToLowerInvariant();

            var category = CategoryId?.ToString() ?? "";

            var min = MinPrice?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            var max = MaxPrice?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";

            var rating = FilterRating?.ToString() ?? "";
            var sort = Sort?.ToString() ?? "";

            return $"k:{keyword}|c:{category}|min:{min}|max:{max}|r:{rating}|s:{sort}|p:{PageIndex}|ps:{PageSize}";
        }

    }
    public enum ProductSort
    {
        Newest,
        PriceAsc,
        PriceDesc,
        DiscountDesc,
        RatingDesc,
        TopSell
    }
}

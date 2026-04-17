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
        public double? Rating { get; set; }
        public string? Sort { get; set; }

        public override string ToString()
        {
            var keyword = string.IsNullOrWhiteSpace(Keyword) ? "" : Keyword.Trim().ToLowerInvariant();

            var category = CategoryId?.ToString() ?? "";

            var min = MinPrice?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            var max = MaxPrice?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";

            var rating = Rating?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            var sort = NormalizeSort(Sort);

            return $"k:{keyword}|c:{category}|min:{min}|max:{max}|r:{rating}|s:{sort}|p:{Page}|ps:{PageSize}";
        }

        public string NormalizeSortValue() => NormalizeSort(Sort);

        private static string NormalizeSort(string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return "";
            }

            return sort.Trim().ToLowerInvariant();
        }
    }
}

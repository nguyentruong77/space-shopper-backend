using SpaceShopper.Application.Requests.Common;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SpaceShopper.Application.Requests.Catalog
{
    public sealed class GetReviewsByProductRequest : PageRequest
    {
        [Required]
        public Guid ProductId { get; set; }
        public string? SortBy { get; set; }

        public string NormalizeSortValue()
        {
            if (string.IsNullOrWhiteSpace(SortBy))
            {
                return "latest";
            }

            return SortBy.Trim().ToLowerInvariant();
        }

        public override string ToString()
        {
            return string.Create(
                CultureInfo.InvariantCulture,
                $"pid:{ProductId}|p:{Page}|ps:{PageSize}|s:{NormalizeSortValue()}");
        }
    }

    public sealed class AddReviewRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}

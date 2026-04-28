using System.Globalization;
using SpaceShopper.Application.Requests.Common;

namespace SpaceShopper.Application.Requests.Orders
{
    public sealed class OrderFilterRequest : PageRequest
    {
        public string? Status { get; set; }

        public override string ToString()
        {
            var normalizedStatus = string.IsNullOrWhiteSpace(Status)
                ? string.Empty
                : Status.Trim().ToLowerInvariant();

            return string.Create(
                CultureInfo.InvariantCulture,
                $"status:{normalizedStatus}|p:{Page}|ps:{PageSize}");
        }
    }

    public sealed class OrderStatusCountQuery
    {
        public string Status { get; set; } = string.Empty;
    }
}

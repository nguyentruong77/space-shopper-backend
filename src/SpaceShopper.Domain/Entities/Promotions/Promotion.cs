using SpaceShopper.Domain.Common;

namespace SpaceShopper.Domain.Entities.Promotions
{
    /// <summary>
    /// Mã khuyến mãi (master), bảng Promotion — logic phụ, chỉ <see cref="BaseEntity"/>.
    /// Bật/tắt bằng <see cref="IsActive"/> (cột mới cần migration nếu DB chưa có).
    /// </summary>
    public class Promotion : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Kiểu giảm: ví dụ Percent, FixedAmount.
        /// </summary>
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsShippingDiscount { get; set; } = false;
    }
}

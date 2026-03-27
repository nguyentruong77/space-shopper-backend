namespace SpaceShopper.Application.Dtos.Dev
{
    public class ExternalCategoryResponse
    {
        public List<CloneCategoryDto> data { get; set; } = new();
    }
    public class CloneCategoryDto
    {
        public string? _id { get; set; }
        public int id { get; set; }
        public long parent_id { get; set; }
        public int position { get; set; }
        public int status { get; set; }
        public string? title { get; set; }
        public string? slug { get; set; }
    }
    public class ExternalProductResponse
    {
        public List<CloneProductDto> data { get; set; } = new();
        public PaginateDto paginate { get; set; }
    }
    public class ExternalReviewProductResponse
    {
        public List<CloneReviewProductDto> data { get; set; } = new();
    }
    public class CloneReviewProductDto
    {
        public string? _id { get; set; }
        public string? orderId { get; set; }
        public int productId { get; set; }
        public string? senderId { get; set; }
        public long createdAt { get; set; }
        public string? content { get; set; }
        public double star { get; set; }
        public UserReviewCloneDto? user { get; set; }
    }
    public class UserReviewCloneDto
    {
        public string? _id { get; set; }
        public string? name { get; set; }
        public string? avatar { get; set; }
    }
    public class Image
    {
        public string? label { get; set; }
        public int? position { get; set; }
        public string? base_url { get; set; }
        public string? thumbnail_url { get; set; }
        public string? small_url { get; set; }
        public string? medium_url { get; set; }
        public string? large_url { get; set; }
        public bool is_gallery { get; set; }
    }
    public class StockItem
    {
        public int qty { get; set; }
        public int min_sale_qty { get; set; }
        public int max_sale_qty { get; set; }
    }
    public class CloneProductDto
    {
        public string? _id { get; set; }
        public long id { get; set; }
        public int categories { get; set; }
        public string? name { get; set; }
        public string? slug { get; set; }
        public string? thumbnail_url { get; set; }
        public string? description { get; set; }
        public string? short_description { get; set; }
        public decimal price { get; set; }
        public decimal real_price { get; set; }
        public decimal discount_rate { get; set; }
        public decimal rating_average { get; set; }
        public int review_count { get; set; }
        public StockItem stock_item { get; set; }
        public List<Image> images { get; set; } = new();
    }
    public class PaginateDto
    {
        public int currentPage { get; set; }
        public int totalPage { get; set; }
        public int count { get; set; }
        public int perPage { get; set; }
        public int nextPage { get; set; }
    }
}

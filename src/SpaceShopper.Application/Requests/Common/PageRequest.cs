namespace SpaceShopper.Application.Requests.Common
{
    public abstract class PageRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;

        public int Skip => Page > 1 ? (Page - 1) * PageSize : 0;
    }
}

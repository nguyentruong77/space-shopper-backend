namespace SpaceShopper.Application.Requests.Common
{
    public abstract class PageRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;

        public int Skip => PageIndex > 1 ? (PageIndex - 1) * PageSize : 0;
    }
}

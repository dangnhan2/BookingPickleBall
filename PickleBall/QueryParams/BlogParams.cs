using PickleBall.Enum;

namespace PickleBall.QueryParams
{
    public class BlogParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Title { get; set; }
        public BlogStatus? Status { get; set; }
    }
}

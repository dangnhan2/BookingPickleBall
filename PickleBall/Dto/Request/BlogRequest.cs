using PickleBall.Models.Enum;

namespace PickleBall.Dto.Request
{
    public class BlogRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid ParnerID { get; set; }
        public IFormFile? ThumbnailUrl { get; set; }
        public BlogStatus BlogStatus { get; set; }
    }
}

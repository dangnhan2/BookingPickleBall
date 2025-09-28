using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class Blog
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ThumbnailUrl { get; set; }
        public Guid PartnerID { get; set; }
        public Partner Partner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public BlogStatus BlogStatus { get; set; }
    }
}

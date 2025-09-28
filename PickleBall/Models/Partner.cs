using Microsoft.AspNetCore.Identity;
using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class Partner : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string BussinessName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string PhoneNumber { get; set; }
        public bool IsApproved { get; set; } 
        public bool IsAdmin { get; set; }
        public string Avatar { get; set; }
        public RefreshTokens RefreshTokens { get; set; }
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<Court> Courts { get; set; } = new List<Court>();
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    }
}

using Microsoft.AspNetCore.Identity;
using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class Partner : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string? BussinessName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } 
        public bool IsAdmin { get; set; }
        public string Avatar { get; set; }
        public string? PayOSClientId { get; set; }
        public string? PayOSApiKey { get; set; }
        public string? PayOSCheckSumKey { get; set; }
        public RefreshTokens RefreshTokens { get; set; }
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<Court> Courts { get; set; } = new List<Court>();
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

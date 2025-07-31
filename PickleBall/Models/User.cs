using Microsoft.AspNetCore.Identity;
using PickleBall.Models.Enum;

namespace PickleBall.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
        public string Avatar { get; set; }
        public RefreshTokens RefreshTokens { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Blog> Blogs { get; set; } = new List<Blog>();
    }
}

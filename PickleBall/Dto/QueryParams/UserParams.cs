using PickleBall.Models.Enum;

namespace PickleBall.Dto.QueryParams
{
    public class UserParams
    {   
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? FullName { get; set; }
        public UserStatus? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}

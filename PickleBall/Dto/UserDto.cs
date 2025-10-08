using PickleBall.Models.Enum;

namespace PickleBall.Dto
{
    public class UserDto
    {   
        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BussinessName { get; set; }
        public string Avatar { get; set; }
        public bool IsApproved { get; set; }
        public string Role { get; set; }
    }
}

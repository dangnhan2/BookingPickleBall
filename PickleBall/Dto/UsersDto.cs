using PickleBall.Enum;

namespace PickleBall.Dto
{
    public class UsersDto
    {   
        public string ID { get; set; }
        public string FullName { get; set; }
        public UserStatus Status { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }
}

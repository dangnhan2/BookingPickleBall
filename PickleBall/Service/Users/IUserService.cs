using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service.Users
{
    public interface IUserService 
    {
        public Task<DataReponse<UsersDto>> GetAll(UserParams user);
        public Task<Result<UsersDto>> GetById(string id);
        public Task<Result<string>> UpdateByUser(string userId, UserRequest user);
        public Task UploadAvatarByUser(string userId, IFormFile file);
        public Task<Result<string>> UpdateByAdmin(string userId, UserRequestByAdmin user);
    }
}

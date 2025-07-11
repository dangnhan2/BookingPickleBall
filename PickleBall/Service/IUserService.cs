using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.QueryParams;

namespace PickleBall.Service
{
    public interface IUserService 
    {
        public Task<DataReponse<UsersDto>> GetAll(UserParams user);
        public Task<UsersDto> GetById(string id);
        public Task UpdateByUser(string userId, UserRequest user);
        public Task UploadAvatarByUser(string userId, IFormFile file);
        public Task UpdateByAdmin(string userId, UserRequestByAdmin user);
    }
}

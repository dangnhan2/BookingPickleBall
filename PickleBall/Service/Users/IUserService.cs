using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service.Users
{
    public interface IUserService 
    {
        public Task<DataReponse<UserDto>> GetAll(UserParams user);
        public Task<Result<UserDto>> GetById(Guid id);
        public Task<Result<string>> UpdateByUser(Guid id, UserRequest user);
        public Task UploadAvatarByUser(Guid Id, IFormFile file);
        
    }
}

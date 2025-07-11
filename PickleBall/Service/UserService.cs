using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.QueryParams;
using PickleBall.Service.SoftService;
using PickleBall.UnitOfWork;

namespace PickleBall.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private readonly string folder = "Avatar";

        public UserService(IUnitOfWorks unitOfWorks, ICloudinaryService cloudinaryService)
        {
            _unitOfWorks = unitOfWorks;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<DataReponse<UsersDto>> GetAll(UserParams user)
        {
            var users = _unitOfWorks.User.Get().AsNoTracking();

            if (!string.IsNullOrEmpty(user.FullName))
            {
                users = users.Where(u => u.FullName.ToLower().Trim().Contains(user.FullName.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                users = users.Where(u => u.PhoneNumber.Contains(user.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                users = users.Where(u => u.Email.Trim().Contains(user.Email.Trim()));
            }

            if (user.Status.HasValue)
            {
                users = users.Where(u => u.Status == user.Status);
            }

            var usersToDto = users.Select(u => new UsersDto
            {
                ID = u.Id,
                FullName = u.FullName,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                Status = u.Status
            }).AsNoTracking().Paging(user.Page, user.PageSize);

            return new DataReponse<UsersDto>
            {
                Page = user.Page,
                PageSize = user.PageSize,
                Total = users.Count(),
                Data = await usersToDto.ToListAsync(),
            };
        }

        public async Task<UsersDto> GetById(string userId)
        {
            var isExistUser = await _unitOfWorks.User.GetById(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            var userToDto = new UsersDto
            {
                ID = isExistUser.Id,
                FullName = isExistUser.FullName,
                UserName = isExistUser.UserName,
                Email = isExistUser.Email,
                PhoneNumber = isExistUser.PhoneNumber,
                Avatar = isExistUser.Avatar,
                Status = isExistUser.Status
            };

            return userToDto;
        }

        public async Task UpdateByAdmin(string userId, UserRequestByAdmin user)
        {
            var isExistUser = await _unitOfWorks.User.GetById(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            isExistUser.Status = user.Status;

            _unitOfWorks.User.Update(isExistUser);
            await _unitOfWorks.CompleteAsync();
        }

        public async Task UpdateByUser(string userId, UserRequest user)
        {
            //var isExistUser = await _unitOfWorks.User.GetById(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            var users = _unitOfWorks.User.Get();
            var isExistUser = await users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            if (await users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber && u.Id != userId))
            {
                throw new ArgumentException("Số điện thoại đã được đăng kí");
            }

            isExistUser.FullName = user.FullName;
            isExistUser.UserName = user.UserName;
            isExistUser.PhoneNumber = user.PhoneNumber;

            _unitOfWorks.User.Update(isExistUser);
            await _unitOfWorks.CompleteAsync();
        }

        public async Task UploadAvatarByUser(string userId, IFormFile file)
        {
            var isExistUser = await _unitOfWorks.User.GetById(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");
            var avatarUrl = await _cloudinaryService.Upload(file, allowedExtension, folder);

            if (isExistUser.Avatar == "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg")
            {
                isExistUser.Avatar = avatarUrl;
            }

            await _cloudinaryService.Delete(isExistUser.Avatar);

            isExistUser.Avatar = avatarUrl;

            _unitOfWorks.User.Update(isExistUser);
            await _unitOfWorks.CompleteAsync();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.Service.Storage;
using PickleBall.UnitOfWork;
using System.Security.Claims;

namespace PickleBall.Service.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private const string folder = "Avatar";

        public UserService(IUnitOfWorks unitOfWorks, ICloudinaryService cloudinaryService)
        {
            _unitOfWorks = unitOfWorks;
            _cloudinaryService = cloudinaryService;        
        }

        public async Task<DataReponse<UserDto>> GetAll(UserParams user)
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


            var usersToDto = users.Where(u => !u.IsAdmin).Select(u => new UserDto
            {
                ID = u.Id,
                FullName = u.FullName,
                BussinessName = u.BussinessName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                IsApproved = u.IsApproved,
                Address = u.Address
            }).Paging(user.Page, user.PageSize);

            return new DataReponse<UserDto>
            {
                Page = user.Page,
                PageSize = user.PageSize,
                Total = users.Count(),
                Data = await usersToDto.ToListAsync(),
            };
        }

        public async Task<ApiResponse<UserDto>> GetById(Guid id)
        {
            var isExistUser = await _unitOfWorks.User.GetById(id); 

            if (isExistUser == null)
            {
                return ApiResponse<UserDto>.Fail("Không tìm thấy người dùng", StatusCodes.Status404NotFound);
            }

            var userToDto = new UserDto
            {
                ID = isExistUser.Id,
                FullName = isExistUser.FullName,
                BussinessName = isExistUser.BussinessName,
                Email = isExistUser.Email,
                PhoneNumber = isExistUser.PhoneNumber,
                Avatar = isExistUser.Avatar,
                IsApproved = isExistUser.IsApproved
            };

            return ApiResponse<UserDto>.Ok(userToDto, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<string>> UpdateByUser(Guid userId, UserRequest user)
        {
            //var isExistUser = await _unitOfWorks.User.GetById(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            var users = _unitOfWorks.User.Get();
            var isExistUser = await users.FirstOrDefaultAsync(u => u.Id == userId);

            if (isExistUser == null)
            {
                return ApiResponse<string>.Fail("Không tìm thấy người dùng", StatusCodes.Status404NotFound);
            }

            if (await users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber && u.Id != userId))
            {
                return ApiResponse<string>.Fail("Số điện thoại đã được đăng kí", StatusCodes.Status404NotFound);
            }

            isExistUser.FullName = user.FullName;
            //isExistUser.UserName = user.UserName;
            isExistUser.PhoneNumber = user.PhoneNumber;

            _unitOfWorks.User.Update(isExistUser);
            await _unitOfWorks.CompleteAsync();

            return ApiResponse<string>.Ok("Cập nhật thành công", StatusCodes.Status200OK);
        }

        public async Task UploadAvatarByUser(Guid id, IFormFile file)
        {
            var isExistUser = await _unitOfWorks.User.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy người dùng");
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

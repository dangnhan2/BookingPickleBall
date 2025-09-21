using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.Service.Storage;
using PickleBall.UnitOfWork;
using PickleBall.Validation;

namespace PickleBall.Service.Blogs
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private const string folder = "Thumbnail";

        public BlogService(IUnitOfWorks unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result<string>> Create(BlogRequest request)
        {   
            var validator = new BlogRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var blogs = _unitOfWork.Blog.Get();

            var newBlog = new Blog
            {
                Title = request.Title,
                Content = request.Content,
                UserID = request.UserID,
                BlogStatus = request.BlogStatus,
                CreatedAt = DateTime.UtcNow
            };

            var thumbnailUrl = await _cloudinaryService.Upload(request.ThumbnailUrl, allowedExtension, folder);

            newBlog.ThumbnailUrl = thumbnailUrl;

            await _unitOfWork.Blog.CreateAsync(newBlog);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Thêm blog thành công", StatusCodes.Status201Created);
           
        }

        public async Task<Result<string>> Delete(Guid id)
        {
            var isExistBlog = await _unitOfWork.Blog.GetById(id); 
            
            if (isExistBlog == null)
            {
                return Result<string>.Fail("Không tìm thấy blog", StatusCodes.Status404NotFound);
            }

            isExistBlog.IsDeleted = true;

            _unitOfWork.Blog.Update(isExistBlog);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Xóa blog thành công", StatusCodes.Status200OK);
        }

        public async Task<DataReponse<BlogDto>> GetAll(BlogParams blog)
        {
            var blogs = _unitOfWork.Blog.Get();

            if (!string.IsNullOrEmpty(blog.Title))
            {
                blogs = blogs.Where(b => b.Title.ToLower().Trim().Contains(blog.Title.ToLower().Trim()));
            }

            if (blog.Status.HasValue)
            {
                blogs = blogs.Where(b => b.BlogStatus == blog.Status);
            }

            var blogsToDto = blogs.Select(b => new BlogDto
            {
                ID = b.ID,
                Title = b.Title,
                Content = b.Content,
                ThumbnailUrl = b.ThumbnailUrl,
                Status = b.BlogStatus,
                UpdatedAt = b.UpdatedAt,
                CreatedAt = b.CreatedAt,
            }).AsNoTracking().Paging(blog.Page, blog.PageSize);

            return new DataReponse<BlogDto>
            {
                Page = blog.Page,
                PageSize = blog.PageSize,
                Total = blogs.Count(),
                Data = await blogsToDto.ToListAsync()
            };
        }

        public async Task<Result<BlogDto>> GetById(Guid id)
        {
            var blog = await _unitOfWork.Blog.GetById(id);

            if(blog == null)
            {
                return Result<BlogDto>.Fail("Không tìm thấy blog", StatusCodes.Status404NotFound);
            }

            var blogToDto = new BlogDto
            {
                ID = blog.ID,
                Title = blog.Title,
                Content = blog.Content,
                ThumbnailUrl = blog.ThumbnailUrl,
                Status = blog.BlogStatus,
                UpdatedAt = blog.UpdatedAt,
                CreatedAt = blog.CreatedAt,
            };

            return Result<BlogDto>.Ok(blogToDto, StatusCodes.Status200OK);
        }

        public async Task<Result<string>> Update(Guid id, BlogRequest request)
        {
            var validator = new BlogRequestValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var blogs = _unitOfWork.Blog.Get();

            var isExistBlog = await _unitOfWork.Blog.GetById(id);

            if (isExistBlog == null)
            {
                return Result<string>.Fail("Không tìm thấy blog", StatusCodes.Status404NotFound);
            }

            if (request.ThumbnailUrl != null || request?.ThumbnailUrl?.Length > 0)
            {
                await _cloudinaryService.Delete(isExistBlog.ThumbnailUrl);

                var thumbnailUrl = await _cloudinaryService.Upload(request.ThumbnailUrl, allowedExtension, folder);

                isExistBlog.ThumbnailUrl = thumbnailUrl;
            }

            isExistBlog.Title = request.Title;
            isExistBlog.Content = request.Content;
            isExistBlog.UserID = request.UserID;
            isExistBlog.BlogStatus = request.BlogStatus;
            isExistBlog.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Blog.Update(isExistBlog);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Cập nhật thành công", StatusCodes.Status200OK);
        }
    }
}

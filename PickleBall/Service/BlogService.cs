using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Extension;
using PickleBall.Models;
using PickleBall.QueryParams;
using PickleBall.Service.SoftService;
using PickleBall.UnitOfWork;

namespace PickleBall.Service
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string[] allowedExtension = { ".jpg", ".png", ".jpeg", };
        private readonly string folder = "Thumbnail";

        public BlogService(IUnitOfWorks unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task Create(BlogRequest request)
        {
            var blogs = _unitOfWork.Blog.Get();

            if (await blogs.AnyAsync(b => b.Title.ToLower().Trim() == request.Title.ToLower().Trim()))
            {
                throw new ArgumentException("Blog đã tồn tại");
            }

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
        }

        public async Task Delete(Guid id)
        {
            var isExistBlog = await _unitOfWork.Blog.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy blog");

            isExistBlog.IsDeleted = true;

            _unitOfWork.Blog.Update(isExistBlog);
            await _unitOfWork.CompleteAsync();
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

        public async Task<BlogDto> GetById(Guid id)
        {
            var blog = await _unitOfWork.Blog.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy blog");

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

            return blogToDto;
        }

        public async Task Update(Guid id, BlogRequest request)
        {
            var blogs = _unitOfWork.Blog.Get();

            if (await blogs.AnyAsync(b => b.Title.ToLower().Trim() == request.Title.ToLower().Trim() && b.ID != id))
            {
                throw new ArgumentException("Blog đã tồn tại");
            }

            var isExistBlog = await _unitOfWork.Blog.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy blog");

            if (request.ThumbnailUrl != null || request?.ThumbnailUrl?.Length > 0)
            {
                await _cloudinaryService.Delete(isExistBlog.ThumbnailUrl);

                var thumbnailUrl = await _cloudinaryService.Upload(request.ThumbnailUrl, allowedExtension, folder);

                isExistBlog.Title = request.Title;
                isExistBlog.Content = request.Content;
                isExistBlog.UserID = request.UserID;
                isExistBlog.ThumbnailUrl = thumbnailUrl;
                isExistBlog.BlogStatus = request.BlogStatus;
                isExistBlog.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Blog.Update(isExistBlog);
                await _unitOfWork.CompleteAsync();
            }

            isExistBlog.Title = request.Title;
            isExistBlog.Content = request.Content;
            isExistBlog.UserID = request.UserID;
            isExistBlog.BlogStatus = request.BlogStatus;
            isExistBlog.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Blog.Update(isExistBlog);
            await _unitOfWork.CompleteAsync();
        }
    }
}

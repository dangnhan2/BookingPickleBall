using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service.Blogs
{
    public interface IBlogService
    {
        public Task<DataReponse<BlogDto>> GetAll(BlogParams blog);
        public Task<ApiResponse<BlogDto>> GetById(Guid id);
        public Task<ApiResponse<string>> Create(BlogRequest request);
        public Task<ApiResponse<string>> Update(Guid id, BlogRequest request);
        public Task<ApiResponse<string>> Delete(Guid id);
    }
}

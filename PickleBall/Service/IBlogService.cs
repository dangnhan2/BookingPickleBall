using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service
{
    public interface IBlogService
    {
        public Task<DataReponse<BlogDto>> GetAll(BlogParams blog);
        public Task<Result<BlogDto>> GetById(Guid id);
        public Task<Result<string>> Create(BlogRequest request);
        public Task<Result<string>> Update(Guid id, BlogRequest request);
        public Task<Result<string>> Delete(Guid id);
    }
}

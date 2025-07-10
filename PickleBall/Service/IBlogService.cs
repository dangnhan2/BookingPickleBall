using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.QueryParams;

namespace PickleBall.Service
{
    public interface IBlogService
    {
        public Task<DataReponse<BlogDto>> GetAll(BlogParams blog);
        public Task<BlogDto> GetById(Guid id);
        public Task Create(BlogRequest request);
        public Task Update(Guid id, BlogRequest request);
        public Task Delete(Guid id);
    }
}

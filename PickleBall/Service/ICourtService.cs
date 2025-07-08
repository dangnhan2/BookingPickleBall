using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.QueryParams;

namespace PickleBall.Service
{
    public interface ICourtService
    {
        public Task<DataReponse<CourtDto>> GetAll(CourtParams court);
        public Task<CourtDto> GetById(Guid id);
        public Task Add(CourtRequest court);
        public Task Update(Guid id, CourtRequest court);
        public Task Delete(Guid id);
    }
}

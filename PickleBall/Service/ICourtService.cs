using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service
{
    public interface ICourtService
    {
        public Task<DataReponse<CourtDto>> GetAll(CourtParams court);
        public Task<Result<CourtDto>> GetById(Guid id);
        public Task<Result<string>> Add(CourtRequest court);
        public Task<Result<string>> Update(Guid id, CourtRequest court);
        public Task<Result<string>> Delete(Guid id);
    }
}

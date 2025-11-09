using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service.Courts
{
    public interface ICourtService
    {
        public Task<DataReponse<CourtDto>> GetAllByPartner(Guid id, CourtParams court);
        public Task<IEnumerable<CourtDto>> GetAllInSpecificDate(Guid id, DateOnly date);
        public Task<ApiResponse<CourtDto>> GetById(Guid id);
        public Task<ApiResponse<string>> Add(CourtRequest court);
        public Task<ApiResponse<string>> Update(Guid id, CourtRequest court);
        public Task<ApiResponse<string>> Delete(Guid id);
        public Task<IEnumerable<UserDto>> GetAllPartnerInfo();
    }
}

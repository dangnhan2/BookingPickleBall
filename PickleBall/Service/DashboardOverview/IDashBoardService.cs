using PickleBall.Dto;

namespace PickleBall.Service.DashboardOverview
{
    public interface IDashBoardService
    {
        public Task<ApiResponse<DashboardDto>> DashboardOverviewByPartner(Guid id);
    }
}

using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.UnitOfWork;

namespace PickleBall.Service.DashboardOverview
{
    public class DashboardOverview : IDashBoardService
    {   
        private readonly IUnitOfWorks _unitOfWorks;
        public DashboardOverview(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }
        public async Task<ApiResponse<DashboardDto>> DashboardOverviewByPartner(Guid id)
        {
            var existPartner = await _unitOfWorks.User.GetById(id);

            if (existPartner == null)
            {
                return ApiResponse<DashboardDto>.Fail("Partner không tồn tại", StatusCodes.Status404NotFound);   
            }

            var bookings = _unitOfWorks.Booking.GetAllByPartner(id);
            var courts = _unitOfWorks.Court.GetAllByPartner(id);
            var totalRevenue = await bookings.Where(b => b.BookingStatus == Models.Enum.BookingStatus.Paid).SumAsync(b => b.TotalAmount);

            var dashboardToDto = new DashboardDto
            {
                TotalRevenue = totalRevenue,
                Courts = courts.Count(),
                TotalBookings = bookings.Count(),
                PaidBookings = bookings.Where(b => b.BookingStatus == Models.Enum.BookingStatus.Paid).Count(),
                CancelledBookings = bookings.Where(b => b.BookingStatus == Models.Enum.BookingStatus.Cancelled).Count(),
            };

            return ApiResponse<DashboardDto>.Ok(dashboardToDto, StatusCodes.Status200OK);
        }
    }
}

using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service.Bookings
{
    public interface IBookingService
    {
        public Task<DataReponse<BookingDto>> GetByPartner(Guid id, BookingParams bookingParams);
    }
}

using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Dto.Request;

namespace PickleBall.Service
{
    public interface IBookingService
    {
        public Task<DataReponse<BookingDto>> Get(BookingParams bookingParams);
    }
}

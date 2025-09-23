using PickleBall.Dto;
using PickleBall.Dto.Request;

namespace PickleBall.Service.Checkout
{
    public interface ICheckoutService
    {
        public Task<Result<dynamic>> Checkout(BookingRequest booking);
    }
}

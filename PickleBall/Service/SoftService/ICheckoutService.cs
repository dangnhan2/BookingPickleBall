using PickleBall.Dto.Request;

namespace PickleBall.Service.SoftService
{
    public interface ICheckoutService
    {
        public Task<object> Checkout(BookingRequest booking);
    }
}

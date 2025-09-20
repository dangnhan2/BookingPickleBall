using PickleBall.Dto.Request;

namespace PickleBall.Service.Checkout
{
    public interface ICheckoutService
    {
        public Task<object> Checkout(BookingRequest booking);
    }
}

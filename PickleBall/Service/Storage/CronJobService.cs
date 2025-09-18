using PickleBall.UnitOfWork;
using Serilog;

namespace PickleBall.Service.Storage
{
    public class CronJobService : ICronJobService
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public CronJobService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task CheckExpiredBoookings()
        {
            var now = DateTime.UtcNow;

            try
            {
                var unpaidBookings = _unitOfWorks.Booking.Get().Where(b => b.BookingStatus == Models.Enum.BookingStatus.Pending
                    && b.PaymentStatus == Models.Enum.PaymentStatus.Unpaid).ToList();

                foreach (var unpaidBooking in unpaidBookings)
                {
                    var booking = await _unitOfWorks.Booking.GetById(unpaidBooking.ID);

                    var createdAt = booking.CreatedAt;

                    if ((now - createdAt).TotalMinutes > 2)
                    {
                        booking.BookingStatus = Models.Enum.BookingStatus.Cancelled;
                        booking.PaymentStatus = Models.Enum.PaymentStatus.Failed;

                        var payment =  await _unitOfWorks.Payment.GetById(booking.Payments.ID);

                        payment.PaymentStatus = Models.Enum.PaymentStatus.Expired;

                        var timeSlotIdsToRemove = booking.BookingTimeSlots
                           .Select(bts => bts.TimeSlotId)
                           .ToList();

                        // Lấy court kèm theo CourtTimeSlots
                        var court = await _unitOfWorks.Court
                            .GetById(booking.CourtID);
                           

                        if (court != null)
                        {
                            var timeslots = await _unitOfWorks.CourtTimeSlot.FindAsyncByCourtId(court.ID);

                            _unitOfWorks.CourtTimeSlot.RemoveRange(timeslots);
                        }

                        _unitOfWorks.Payment.Update(payment);
                        _unitOfWorks.Booking.Update(booking);
                    }
                }

                
                await _unitOfWorks.CompleteAsync();

            }catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            
        }

        public async Task DeleteExpiredRefreshToken()
        {
            var now = DateTime.UtcNow;

            var expiredRefreshToken = _unitOfWorks.RefreshToken.Get();

            try
            {
                foreach (var token in expiredRefreshToken)
                {

                    var expiredAt = token.ExpiresAt;

                    if (now > expiredAt)
                    {
                        _unitOfWorks.RefreshToken.Delete(token);
                    }
                }

                await _unitOfWorks.CompleteAsync();
            }catch(Exception ex)
            {
                Log.Error($"Lỗi xóa refreshToken : ${ex.InnerException.Message ?? ex.Message}");
            }
           
        }
    }
}

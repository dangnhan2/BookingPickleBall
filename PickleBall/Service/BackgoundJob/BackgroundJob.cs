using Microsoft.AspNetCore.SignalR;
using PickleBall.Models.Enum;
using PickleBall.Service.SignalR;
using PickleBall.UnitOfWork;
using Serilog;

namespace PickleBall.Service.BackgoundJob
{
    public class BackgroundJob : IBackgroundJob
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IHubContext<BookingHub> _hubContext;

        public BackgroundJob(IUnitOfWorks unitOfWorks, IHubContext<BookingHub> hubContext)
        {
            _unitOfWorks = unitOfWorks;
            _hubContext = hubContext;
        }

        public async Task CheckAndReleaseExpiredBookings()
        {
            var expiredBookings = await _unitOfWorks.Booking.GetExpiredBookings();

            try
            {
                foreach (var booking in expiredBookings)
                {
                    booking.BookingStatus = BookingStatus.Cancelled;

                    foreach(var bt in booking.BookingTimeSlots)
                    {
                        var slot = bt.TimeSlot;

                        var payload = new SlotEventPayload
                        {
                            CourtId = booking.CourtID,
                            BookingDate = booking.BookingDate,
                            TimeSlotId = slot.ID,
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime,
                            Status = SlotStatus.Released,
                        };

                        await _hubContext.Clients.Group(booking.CourtID.ToString())
                       .SendAsync("SlotReleased", payload);

                    }

                    _unitOfWorks.Booking.DeleteExpiredBookingTimeSlot(booking.BookingTimeSlots);
                }
                
                if(expiredBookings.Count() > 0)
                {     
                   await _unitOfWorks.CompleteAsync();
                }

            }catch(Exception ex)
            {
                Log.Error($"Lỗi xóa refreshToken : ${ex.InnerException.Message ?? ex.Message}");
            }
            
        }

        public async Task DeleteExpiredRefreshToken()
        {
            var expiredRefreshToken = await _unitOfWorks.RefreshToken.GetExpiredRefreshToken();

            try
            {
                if (expiredRefreshToken.Count() > 0)
                {   
                    _unitOfWorks.RefreshToken.RemoveRefreshTokens(expiredRefreshToken);
                    await _unitOfWorks.CompleteAsync();
                }
                
            }catch(Exception ex)
            {
                Log.Error($"Lỗi xóa refreshToken : ${ex.InnerException.Message ?? ex.Message}");
            }
           
        }
    }
}

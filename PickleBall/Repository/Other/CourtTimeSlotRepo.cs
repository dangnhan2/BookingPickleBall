using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Other
{
    public interface ICourtTimeSlotRepo
    {
        public Task<IEnumerable<CourtTimeSlot>> FindAsyncByCourtId(Guid courtId);
        public void RemoveRange(IEnumerable<CourtTimeSlot> mappings);
    }

    public class CourtTimeSlotRepo : ICourtTimeSlotRepo
    {
        private readonly BookingContext _bookingContext;

        public CourtTimeSlotRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public void RemoveRange(IEnumerable<CourtTimeSlot> mappings)
        {
            _bookingContext.CourtTimeSlots.RemoveRange(mappings);
        }

        public async Task<IEnumerable<CourtTimeSlot>> FindAsyncByCourtId(Guid courtId)
        {
            return await _bookingContext.CourtTimeSlots
                .Include(cts => cts.TimeSlot)
                .Where(cts => cts.CourtID == courtId)
                .ToListAsync();
        }
    }

}

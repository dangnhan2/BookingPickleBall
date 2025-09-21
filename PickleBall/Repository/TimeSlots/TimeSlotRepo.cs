using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.TimeSlots
{
    public interface ITimeSlotRepo
    {
        public IQueryable<TimeSlot> Get();
        public Task<TimeSlot?> GetById(Guid id);
        public Task CreateAsync(TimeSlot timeSlot);
        public void Update(TimeSlot timeSlot);
        public void Delete(TimeSlot timeSlot);
    }

    public class TimeSlotRepo : ITimeSlotRepo
    {
        private readonly BookingContext _bookingContext;

        public TimeSlotRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public async Task CreateAsync(TimeSlot timeSlot)
        {
            await _bookingContext.TimeSlots.AddAsync(timeSlot);
        }

        public void Delete(TimeSlot timeSlot)
        {
            _bookingContext.TimeSlots.Remove(timeSlot);
        }

        public IQueryable<TimeSlot> Get()
        {
            return _bookingContext.TimeSlots.AsQueryable();
        }

        public async Task<TimeSlot?> GetById(Guid id)
        {
            return await _bookingContext.TimeSlots.FirstOrDefaultAsync(tl => tl.ID == id);
        }

        public void Update(TimeSlot timeSlot)
        {
            _bookingContext.TimeSlots.Update(timeSlot);
        }
    }
}

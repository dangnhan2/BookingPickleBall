using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.TimeSlots
{
    public interface ITimeSlotRepo
    {
        public IQueryable<TimeSlot> Get();
        public IQueryable<TimeSlot> GetAllByPartner(Guid id);
        public Task<TimeSlot?> GetById(Guid id);
        public void Create(TimeSlot timeSlot);
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

        public void Create(TimeSlot timeSlot)
        {
            _bookingContext.TimeSlots.Add(timeSlot);
        }

        public void Delete(TimeSlot timeSlot)
        {
            _bookingContext.TimeSlots.Remove(timeSlot);
        }

        public IQueryable<TimeSlot> Get()
        {
            return _bookingContext.TimeSlots.AsQueryable();
        }

        public IQueryable<TimeSlot> GetAllByPartner(Guid id)
        {
            return _bookingContext.TimeSlots.Where(s => s.PartnerId == id).AsQueryable();
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

//{
//    "partnerId": "b3f0c7d0-c7f1-474a-8bd6-66883787c0d7",
//  "startTime": "10:00:00",
//  "endTime": "11:00:00"
//}
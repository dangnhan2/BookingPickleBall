using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Courts
{
    public interface ICourtRepo
    {
        public IQueryable<Court> Get();
        public IQueryable<Court> GetAllByPartner(Guid id);
        public Task<Court?> GetById(Guid id);
        public Task CreateAsync(Court court);
        public void Update(Court court);
        public void Delete(Court court);
    }

    public class CourtRepo : ICourtRepo
    {
        private readonly BookingContext _bookingContext;

        public CourtRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }
        public async Task CreateAsync(Court court)
        {
            await _bookingContext.Courts.AddAsync(court);
        }

        public void Delete(Court court)
        {
           _bookingContext.Courts.Remove(court);
        }

        public void Update(Court court)
        {
            _bookingContext.Courts.Update(court);
        }

        public IQueryable<Court> Get()
        {
            return  _bookingContext.Courts.AsQueryable();
        }

        public async Task<Court?> GetById(Guid id)
        {
            return await _bookingContext.Courts
                .Include(c => c.CourtTimeSlots)
                .ThenInclude(cts => cts.TimeSlot)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public IQueryable<Court> GetAllByPartner(Guid id)
        {
            return _bookingContext.Courts.Where(c => c.PartnerId == id).AsQueryable();
        }
    }
}

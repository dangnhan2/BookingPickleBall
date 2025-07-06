using PickleBall.Models;

namespace PickleBall.Repository
{
    public interface TimeSlotRepo
    {
        public IQueryable<TimeSlot> Get();
        public Task<TimeSlot> GetById(Guid id);
        public Task CreateAsync(Court court);
        public void Update(Court court);
        public void Delete(Court court);
    }
}

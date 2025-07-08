using PickleBall.Dto;
using PickleBall.Dto.Request;

namespace PickleBall.Service
{
    public interface ITimeSlotService
    {
        public Task<IEnumerable<TimeSlotDto>> GetAll();
        public Task Add(TimeSlotRequest timeSlot);
        public Task Delete(Guid id);
    }
}

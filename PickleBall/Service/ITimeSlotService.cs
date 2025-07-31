using PickleBall.Dto;
using PickleBall.Dto.Request;

namespace PickleBall.Service
{
    public interface ITimeSlotService
    {
        public Task<IEnumerable<TimeSlotDto>> GetAll();
        public Task<Result<string>> Add(TimeSlotRequest timeSlot);
        public Task<Result<string>> Delete(Guid id);
    }
}

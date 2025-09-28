using PickleBall.Dto;
using PickleBall.Dto.Request;

namespace PickleBall.Service.TimeSlots
{
    public interface ITimeSlotService
    {
        public Task<IEnumerable<TimeSlotDto>> GetByPartner(Guid partnerId);
        public Task<Result<string>> Add(TimeSlotRequest timeSlot);
        public Task<Result<string>> Delete(Guid id);
    }
}

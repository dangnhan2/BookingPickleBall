using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;
using PickleBall.UnitOfWork;
using System.Threading.Tasks;

namespace PickleBall.Service
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly IUnitOfWorks _unitOfWork;

        public TimeSlotService(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TimeSlotDto>> GetAll()
        {
            var timeSlots = _unitOfWork.TimeSlot.Get();

            var timeSlotsToDto = timeSlots.OrderBy(tl => tl.StartTime).Select(ts => new TimeSlotDto
            {
                ID = ts.ID,
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
            }).AsNoTracking();

            return await timeSlotsToDto.ToListAsync();
        }

        public async Task Add(TimeSlotRequest timeSlot)
        {
            var isExistTimeSlot = _unitOfWork.TimeSlot.Get();

            if(await isExistTimeSlot.AnyAsync(tl => tl.StartTime == timeSlot.StartTime || tl.EndTime == timeSlot.EndTime)){
                throw new ArgumentException("Thời gian bắt đầu hoặc thời gian kết thúc bị trùng lặp với một khung giờ đã tồn tại.");
            }

            var newTimeSlot = new TimeSlot
            {
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
            };

            await _unitOfWork.TimeSlot.CreateAsync(newTimeSlot);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(Guid id)
        {
            var isExistTimeSlot = await _unitOfWork.TimeSlot.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy khung thời gian");

            _unitOfWork.TimeSlot.Delete(isExistTimeSlot);
            await _unitOfWork.CompleteAsync();
        }
    }
}

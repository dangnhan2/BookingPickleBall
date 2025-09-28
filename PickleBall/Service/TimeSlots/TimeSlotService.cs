using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;
using PickleBall.UnitOfWork;
using PickleBall.Validation;

namespace PickleBall.Service.TimeSlots
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly IUnitOfWorks _unitOfWork;

        public TimeSlotService(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Add(TimeSlotRequest timeSlot)
        {
            var validator = new TimeSlotRequestValidator();

            var result = await validator.ValidateAsync(timeSlot);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var isExistTimeSlot = _unitOfWork.TimeSlot.Get();

            if (await isExistTimeSlot.AnyAsync(tl => tl.StartTime == timeSlot.StartTime || tl.EndTime == timeSlot.EndTime))
            {
                return Result<string>.Fail("Thời gian bắt đầu hoặc thời gian kết thúc bị trùng lặp với một khung giờ đã tồn tại.", StatusCodes.Status400BadRequest);
            }

            var newTimeSlot = new TimeSlot
            {
                PartnerId = timeSlot.PartnerId,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
            };

            _unitOfWork.TimeSlot.Create(newTimeSlot);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Thêm mới thành công", StatusCodes.Status201Created);
        }

        public async Task<Result<string>> Delete(Guid id)
        {   
            var isExistTimeSlot = await _unitOfWork.TimeSlot.GetById(id);

            if (isExistTimeSlot == null)
            {
                return Result<string>.Fail("Không tìm thấy khung thời gian", StatusCodes.Status404NotFound);
            }

            _unitOfWork.TimeSlot.Delete(isExistTimeSlot);
            await _unitOfWork.CompleteAsync();

            return Result<string>.Ok("Xóa thành công", StatusCodes.Status200OK);
        }

        public async Task<IEnumerable<TimeSlotDto>> GetByPartner(Guid partnerId)
        {
            var timeSlots =  _unitOfWork.TimeSlot.GetAllByPartner(partnerId);

            var timeSlotsToDto = timeSlots.OrderBy(tl => tl.StartTime).Select(ts => new TimeSlotDto
            {
                ID = ts.ID,
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
            });

            return await timeSlotsToDto.ToListAsync();
        }
    }
}

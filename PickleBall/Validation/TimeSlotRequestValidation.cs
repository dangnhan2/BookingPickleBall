using FluentValidation;
using PickleBall.Dto.Request;

namespace PickleBall.Validation
{
    public class TimeSlotRequestValidation : AbstractValidator<TimeSlotRequest>
    {
        public TimeSlotRequestValidation() {
            RuleFor(x => x.StartTime)
            .NotNull().WithMessage("Thời gian bắt đầu không được để trống.")
            .LessThan(x => x.EndTime).WithMessage("Thời gian bắt đầu phải trước thời gian kết thúc.");

            RuleFor(x => x.EndTime)
                .NotNull().WithMessage("Thời gian kết thúc không được để trống.")
                .GreaterThan(x => x.StartTime).WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");

        }
    }
}

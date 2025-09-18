using FluentValidation;
using PickleBall.Dto.Request;

namespace PickleBall.Validation
{
    public class CourtRequestValidator : AbstractValidator<CourtRequest>
    {
        public CourtRequestValidator() {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên sân không được để trống.")
            .Length(3, 100).WithMessage("Tên sân phải có độ dài từ 3 đến 100 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được để trống.")
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Địa điểm không được để trống.")
                .MaximumLength(200).WithMessage("Địa điểm không được vượt quá 200 ký tự.");

            RuleFor(x => x.PricePerHour)
                .GreaterThan(0).WithMessage("Giá mỗi giờ phải lớn hơn 0.");

            RuleFor(x => x.CourtStatus)
                .IsInEnum().WithMessage("Trạng thái sân không hợp lệ.");
        }

    }
}


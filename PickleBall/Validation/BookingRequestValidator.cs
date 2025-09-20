using FluentValidation;
using PickleBall.Dto.Request;

namespace PickleBall.Validation
{
    public class BookingRequestValidator : AbstractValidator<BookingRequest>
    {
        public BookingRequestValidator()
        {

            // Kiểm tra trường CourtID
            RuleFor(x => x.CourtID)
                .NotEmpty().WithMessage("ID sân không được để trống.");

            // Kiểm tra trường BookingDate
            RuleFor(x => x.BookingDate)
                .NotEmpty().WithMessage("Ngày đặt sân không được để trống.")
                .Must(BeAValidDate).WithMessage("Ngày đặt sân không được ở trong quá khứ.");

            // Kiểm tra trường CustomerName
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Tên khách hàng không được để trống.")
                .MaximumLength(100).WithMessage("Tên khách hàng không được vượt quá 100 ký tự.");

            // Kiểm tra trường Amount
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");

            // Kiểm tra trường TimeSlots
            RuleFor(x => x.TimeSlots)
                .NotEmpty().WithMessage("Bạn phải chọn ít nhất một khung giờ.")
                .Must(list => list.All(id => id != Guid.Empty)).WithMessage("Các khung giờ đã chọn không hợp lệ.");
        }

        private bool BeAValidDate(DateOnly date)
        {
            return date >= DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

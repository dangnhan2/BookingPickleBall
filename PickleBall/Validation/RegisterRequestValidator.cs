using FluentValidation;
using PickleBall.Dto.Request;


namespace PickleBall.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator() {
            RuleFor(x => x.FullName)
             .NotEmpty().WithMessage("Tên đầy đủ không được để trống.")
             .MaximumLength(100).WithMessage("Tên đầy đủ không được vượt quá 100 ký tự.");

            // PhoneNumber: Không được để trống và phải theo định dạng số điện thoại Việt Nam (10 chữ số)
            // Regex này bao gồm các đầu số di động phổ biến của Viettel, MobiFone, VinaPhone, Vietnamobile, Gmobile
            // và cho phép có hoặc không có số 0 ở đầu.
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$")
                .WithMessage("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam gồm 10 chữ số.");

            // Email: Không được để trống và phải là định dạng email hợp lệ
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

            // Password: Yêu cầu độ phức tạp cao để tăng cường bảo mật
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");

            // ConfirmPassword: Phải khớp với Password và không được để trống
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
                .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp với mật khẩu.");

        }
    }
}

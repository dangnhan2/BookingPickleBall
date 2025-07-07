using FluentValidation;
using PickleBall.Dto.Request;


namespace PickleBall.Validation
{
    public class LoginRequestValidation : AbstractValidator<LoginRequest>
    {  
        public LoginRequestValidation() {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Địa chỉ email không được để trống.") // Email không được rỗng
            .EmailAddress().WithMessage("Địa chỉ email không hợp lệ."); // Phải là định dạng email hợp lệ

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.");
        }      
    }
}

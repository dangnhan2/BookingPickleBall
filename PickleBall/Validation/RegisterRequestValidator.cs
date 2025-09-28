using FluentValidation;
using PickleBall.Dto.Request;


namespace PickleBall.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterPartnerRequest>
    {
        public RegisterRequestValidator() {

            // Email: Không được để trống và phải là định dạng email hợp lệ
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

        }
    }
}

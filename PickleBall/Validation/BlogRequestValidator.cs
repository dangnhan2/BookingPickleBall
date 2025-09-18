using FluentValidation;
using PickleBall.Dto.Request;

namespace PickleBall.Validation
{
    public class BlogRequestValidator : AbstractValidator<BlogRequest>
    {
        public BlogRequestValidator() {
            RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Tiêu đề không được để trống.")
              .Length(10, 150).WithMessage("Tiêu đề phải có độ dài từ 10 đến 150 ký tự.");

            // Kiểm tra trường Content
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống.")
                .MinimumLength(50).WithMessage("Nội dung phải có ít nhất 50 ký tự.");

            // Kiểm tra trường BlogStatus
            RuleFor(x => x.BlogStatus)
                .IsInEnum().WithMessage("Trạng thái bài viết không hợp lệ.");
        }
    }
}

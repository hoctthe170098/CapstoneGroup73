using StudyFlow.Application.ChinhSaches.Commands.CreateChinhSach;

namespace StudyFlow.Application.ChinhSachs.Commands.CreateChinhSach;

public class CreateChinhSachValidator : AbstractValidator<CreateChinhSachCommand>
{
    public CreateChinhSachValidator()
    {
        RuleFor(x => x.CreateChinhSachDto.Ten)
            .NotEmpty().WithMessage("Tên chính sách không được để trống.")
            .MaximumLength(30).WithMessage("Tên chính sách không được vượt quá 30 ký tự.");

        RuleFor(x => x.CreateChinhSachDto.Mota)
            .NotEmpty().WithMessage("Mô tả không được để trống.")
            .MaximumLength(200).WithMessage("Mô tả không được vượt quá 200 ký tự.");

        RuleFor(x => x.CreateChinhSachDto.PhanTramGiam)
            .GreaterThan(0f).WithMessage("Phần trăm giảm phải lớn hơn 0.")
            .LessThan(0.1f).WithMessage("Phần trăm giảm phải nhỏ hơn 0.1.");
    }
}

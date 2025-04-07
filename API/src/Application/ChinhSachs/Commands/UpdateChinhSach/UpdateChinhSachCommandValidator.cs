namespace StudyFlow.Application.ChinhSachs.Commands.UpdateChinhSach;

public class UpdateChinhSachValidator : AbstractValidator<UpdateChinhSachCommand>
{
    public UpdateChinhSachValidator()
    {
        RuleFor(x => x.UpdateChinhSachDto.Id)
            .GreaterThan(0).WithMessage("ID chính sách phải là số dương.");

        RuleFor(x => x.UpdateChinhSachDto.Ten)
            .MaximumLength(30).WithMessage("Tên chính sách không được vượt quá 30 ký tự.");

        RuleFor(x => x.UpdateChinhSachDto.Mota)
            .MaximumLength(200).WithMessage("Mô tả không được vượt quá 200 ký tự.");

        RuleFor(x => x.UpdateChinhSachDto.PhanTramGiam)
            .GreaterThan(0f).WithMessage("Phần trăm giảm phải lớn hơn 0.8.")
            .LessThan(0.1f).WithMessage("Phần trăm giảm phải nhỏ hơn 1.")
            .When(x => x.UpdateChinhSachDto.PhanTramGiam.HasValue);
    }
}

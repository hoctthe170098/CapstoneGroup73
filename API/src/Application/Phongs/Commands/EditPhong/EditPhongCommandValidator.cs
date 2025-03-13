using FluentValidation;

namespace StudyFlow.Application.Phongs.Commands.EditPhong;

public class EditPhongCommandValidator : AbstractValidator<EditPhongCommand>
{
    public EditPhongCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0).WithMessage("Id phòng không hợp lệ!");

        RuleFor(v => v.Ten)
            .NotEmpty().WithMessage("Tên phòng không được để trống!")
            .MaximumLength(20).WithMessage("Tên phòng không được dài quá 20 ký tự!");

        RuleFor(v => v.TrangThai)
            .NotEmpty().WithMessage("Trạng thái không được để trống!");

    }
}

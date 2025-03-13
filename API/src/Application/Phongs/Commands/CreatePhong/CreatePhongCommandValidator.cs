using FluentValidation;

namespace StudyFlow.Application.Phongs.Commands.CreatePhong;

public class CreatePhongCommandValidator : AbstractValidator<CreatePhongCommand>
{
    public CreatePhongCommandValidator()
    {
        RuleFor(v => v.Ten)
            .NotEmpty().WithMessage("Tên phòng không được để trống!")
            .MaximumLength(50).WithMessage("Tên phòng không được dài quá 50 ký tự!");

        
    }
}

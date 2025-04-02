
namespace StudyFlow.Application.BaiTaps.Commands.DeleteBaiTap;

public class DeleteBaiTapCommandValidator : AbstractValidator<DeleteBaiTapCommand>
{
    public DeleteBaiTapCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id không được để trống.")
            .Must(id => id != Guid.Empty).WithMessage("Id không hợp lệ.");
    }
}

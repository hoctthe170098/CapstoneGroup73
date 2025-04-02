using FluentValidation;

namespace StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;

public class CreateBaiTapCommandValidator : AbstractValidator<CreateBaiTapCommand>
{
    public CreateBaiTapCommandValidator()
    {
        RuleFor(x => x.CreateBaiTapDto.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(50).WithMessage("Tiêu đề tối đa 50 ký tự.");

        RuleFor(x => x.CreateBaiTapDto.NoiDung)
            .NotEmpty().WithMessage("Nội dung không được để trống.")
            .MaximumLength(750).WithMessage("Nội dung tối đa 750 ký tự.");

        RuleFor(x => x.CreateBaiTapDto.TrangThai)
            .NotEmpty().WithMessage("Trạng thái không được để trống.")
            .MaximumLength(10).WithMessage("Trạng thái tối đa 10 ký tự.");

        RuleFor(x => x.CreateBaiTapDto.UrlFile)
            .MaximumLength(200).WithMessage("Đường dẫn file tối đa 200 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.CreateBaiTapDto.UrlFile));

        RuleFor(x => x.CreateBaiTapDto.LichHocId)
            .NotEmpty().WithMessage("Lịch học không được để trống.");

        RuleFor(x => x.CreateBaiTapDto.Ngay)
            .NotEmpty().WithMessage("Ngày giao bài không được để trống.");

        RuleFor(x => x.CreateBaiTapDto.ThoiGianBatDau)
            .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống.");

        RuleFor(x => x.CreateBaiTapDto.ThoiGianKetThuc)
            .NotEmpty().WithMessage("Thời gian kết thúc không được để trống.");

        RuleFor(x => x)
            .Must(x => x.CreateBaiTapDto.ThoiGianBatDau < x.CreateBaiTapDto.ThoiGianKetThuc)
            .WithMessage("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");
    }
}

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
            .Must(BeValidFileType)
            .When(x => !string.IsNullOrWhiteSpace(x.CreateBaiTapDto.UrlFile))
            .WithMessage("Tệp phải là .pdf, .doc hoặc .docx");

        RuleFor(x => x.CreateBaiTapDto.LichHocId)
            .NotEmpty().WithMessage("Lịch học không được để trống.");

        RuleFor(x => x.CreateBaiTapDto.ThoiGianKetThuc)
            .NotNull().WithMessage("Thời gian kết thúc không được để trống.")
            .Must(BeFutureTime)
            .WithMessage("Thời gian kết thúc phải sau thời điểm hiện tại.");
    }

    private bool BeValidFileType(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var ext = Path.GetExtension(url).ToLower();

        return allowedExtensions.Contains(ext);
    }

    private bool BeFutureTime(DateTime? thoiGianKetThuc)
    {
        if (!thoiGianKetThuc.HasValue) return false;
        return thoiGianKetThuc.Value > DateTime.UtcNow;
    }
}

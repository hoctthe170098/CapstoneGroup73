using FluentValidation;
using System.IO;

namespace StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;

public class UpdateBaiTapCommandValidator : AbstractValidator<UpdateBaiTapCommand>
{
    public UpdateBaiTapCommandValidator()
    {
        RuleFor(x => x.UpdateBaiTapDto.Id)
            .NotEmpty().WithMessage("Id bài tập không được để trống.");

        RuleFor(x => x.UpdateBaiTapDto.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(50).WithMessage("Tiêu đề tối đa 50 ký tự.");

        RuleFor(x => x.UpdateBaiTapDto.NoiDung)
            .NotEmpty().WithMessage("Nội dung không được để trống.")
            .MaximumLength(750).WithMessage("Nội dung tối đa 750 ký tự.");

        RuleFor(x => x.UpdateBaiTapDto.TrangThai)
            .NotEmpty().WithMessage("Trạng thái không được để trống.")
            .MaximumLength(10).WithMessage("Trạng thái tối đa 10 ký tự.");

        RuleFor(x => x.UpdateBaiTapDto.UrlFile)
            .MaximumLength(200).WithMessage("Đường dẫn file tối đa 200 ký tự.")
            .Must(BeValidFileType)
            .When(x => !string.IsNullOrWhiteSpace(x.UpdateBaiTapDto.UrlFile))
            .WithMessage("Tệp phải có định dạng .pdf, .doc hoặc .docx");

        RuleFor(x => x.UpdateBaiTapDto.ThoiGianKetThuc)
            .NotNull().WithMessage("Thời gian kết thúc không được để trống.")
            .Must(BeFutureTime)
            .WithMessage("Thời gian kết thúc phải sau thời điểm hiện tại.");
    }

    private bool BeValidFileType(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var ext = Path.GetExtension(url).ToLowerInvariant();

        return allowedExtensions.Contains(ext);
    }

    private bool BeFutureTime(DateTime? time)
    {
        return time.HasValue && time.Value > DateTime.UtcNow;
    }
}

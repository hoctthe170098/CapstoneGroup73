using FluentValidation;
using Microsoft.AspNetCore.Http;
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
            .Must(BeAllowedStatus)
            .WithMessage("Trạng thái không hợp lệ. Chỉ được phép 'Đang mở', 'Chưa mở' Hoặc 'Kết Thúc'.");

        RuleFor(x => x.UpdateBaiTapDto.TaiLieu)
            .Must(BeValidFile)
            .When(x => x.UpdateBaiTapDto.TaiLieu != null)
            .WithMessage("Tệp phải có định dạng .pdf, .doc hoặc .docx và kích thước tối đa 10MB.");

        RuleFor(x => x.UpdateBaiTapDto.ThoiGianKetThuc)
            .NotNull().WithMessage("Thời gian kết thúc không được để trống.")
            .Must(BeFutureTime)
            .WithMessage("Thời gian kết thúc phải sau thời điểm hiện tại.");
    }

    private bool BeFutureTime(DateTime? time)
    {
        return time.HasValue && time.Value > DateTime.Now;
    }

    private bool BeAllowedStatus(string trangThai)
    {
        var allowed = new[] { "Chưa mở", "Đang mở" , "Kết thúc"};
        return allowed.Contains(trangThai.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    private bool BeValidFile(IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var maxSizeInMB = 10;

        return allowedExtensions.Contains(ext) && file.Length <= maxSizeInMB * 1024 * 1024;
    }
}

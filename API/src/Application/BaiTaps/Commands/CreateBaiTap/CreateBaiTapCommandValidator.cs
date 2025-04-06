using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
using StudyFlow.Application.Common.Interfaces;

public class CreateBaiTapCommandValidator : AbstractValidator<CreateBaiTapCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateBaiTapCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.CreateBaiTapDto.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(50).WithMessage("Tiêu đề tối đa 50 ký tự.");

        RuleFor(x => x.CreateBaiTapDto.NoiDung)
            .NotEmpty().WithMessage("Nội dung không được để trống.")
            .MaximumLength(750).WithMessage("Nội dung tối đa 750 ký tự.");

        RuleFor(x => x.CreateBaiTapDto.ThoiGianKetThuc)
            .NotNull().WithMessage("Thời gian kết thúc không được để trống.")
            .Must(BeFutureTime)
            .WithMessage("Thời gian kết thúc phải sau thời điểm hiện tại.");

        RuleFor(x => x.CreateBaiTapDto.TaiLieu)
            .Must(BeValidFileType)
            .When(x => x.CreateBaiTapDto.TaiLieu != null)
            .WithMessage("Tệp phải có định dạng .pdf, .doc hoặc .docx.");

        RuleFor(x => x)
            .MustAsync(NgayTaoValidWithLichHoc)
            .WithMessage("Hôm nay không có lịch học của lớp hoặc ngày tạo không hợp lệ.");

        RuleFor(x => x)
            .MustAsync(KhongTrungNgayTao)
            .WithMessage("Đã tồn tại bài tập được tạo hôm nay cho lớp học này.");
    }

    private bool BeValidFileType(IFormFile? file)
    {
        if (file == null) return true;
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    private bool BeFutureTime(DateTime? dateTime)
    {
        return dateTime.HasValue && dateTime.Value > DateTime.UtcNow;
    }

    private async Task<bool> NgayTaoValidWithLichHoc(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateBaiTapDto;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thu = ConvertDayOfWeekToThu(DateTime.UtcNow.DayOfWeek);

        var lichHoc = await _context.LichHocs
            .FirstOrDefaultAsync(lh =>
                lh.TenLop == dto.TenLop &&
                lh.Thu == thu &&
                lh.NgayBatDau <= today &&
                today <= lh.NgayKetThuc,
                cancellationToken);

        return lichHoc != null;
    }

    private async Task<bool> KhongTrungNgayTao(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateBaiTapDto;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var trung = await _context.BaiTaps.AnyAsync(bt =>
            bt.LichHoc.TenLop == dto.TenLop &&
            bt.NgayTao == today,
            cancellationToken);

        return !trung;
    }

    private int ConvertDayOfWeekToThu(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => 8,
            DayOfWeek.Monday => 2,
            DayOfWeek.Tuesday => 3,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 5,
            DayOfWeek.Friday => 6,
            DayOfWeek.Saturday => 7,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek))
        };
    }
}

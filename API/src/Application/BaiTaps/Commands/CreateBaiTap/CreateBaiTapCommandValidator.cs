using Microsoft.AspNetCore.Http;
using StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
using StudyFlow.Application.Common.Interfaces;
using System.Linq;

public class CreateBaiTapCommandValidator : AbstractValidator<CreateBaiTapCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateBaiTapCommandValidator(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;

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
            .Must(ValidTaiLieu)
            .When(x => x.CreateBaiTapDto.TaiLieu != null)
            .WithMessage("Tệp phải là .pdf, .doc hoặc .docx và không vượt quá 10MB.");

        RuleFor(x => x)
            .MustAsync(HomNayCoLichHoc)
            .WithMessage("Hôm nay không có lịch học phù hợp để tạo bài tập.");

        RuleFor(x => x)
            .MustAsync(KhongPhaiLichDayThay)
            .WithMessage("Bạn không thể tạo bài tập trong buổi dạy thay.");
    }

    private bool ValidTaiLieu(IFormFile file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        bool isValidExtension = allowedExtensions.Contains(extension);
        bool isValidSize = file.Length <= 10 * 1024 * 1024;

        return isValidExtension && isValidSize;
    }

    private bool BeFutureTime(DateTime? dateTime)
    {
        return dateTime.HasValue && dateTime.Value > DateTime.Now;
    }

    private async Task<bool> HomNayCoLichHoc(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateBaiTapDto;
        var today = DateOnly.FromDateTime(DateTime.Now);
        var thu = ConvertDayOfWeekToThu(DateTime.Now.DayOfWeek);

        var lichHoc = await _context.LichHocs
            .FirstOrDefaultAsync(lh =>
                lh.TenLop == dto.TenLop &&
                lh.Thu == thu &&
                lh.NgayBatDau <= today &&
                today <= lh.NgayKetThuc,
                cancellationToken);

        return lichHoc != null;
    }

    private async Task<bool> KhongPhaiLichDayThay(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateBaiTapDto;
        var today = DateOnly.FromDateTime(DateTime.Now);
        var thu = ConvertDayOfWeekToThu(DateTime.Now.DayOfWeek);

        var lichHoc = await _context.LichHocs
            .FirstOrDefaultAsync(lh =>
                lh.TenLop == dto.TenLop &&
                lh.Thu == thu &&
                lh.NgayBatDau <= today &&
                today <= lh.NgayKetThuc,
                cancellationToken);

        if (lichHoc == null) return true; 

        return !string.Equals(lichHoc.TrangThai?.Trim(), "Dạy thay", StringComparison.OrdinalIgnoreCase);
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

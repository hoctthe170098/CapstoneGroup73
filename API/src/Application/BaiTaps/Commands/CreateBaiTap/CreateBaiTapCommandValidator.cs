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

        RuleFor(x => x)
            .MustAsync(NgayTaoValidWithLichHoc)
            .WithMessage("Ngày tạo không hợp lệ với lịch học (ngoài phạm vi hoặc sai thứ).");

        RuleFor(x => x)
            .MustAsync(KhongTrungNgayTao)
            .WithMessage("Đã tồn tại bài tập được tạo hôm nay cho lớp học này.");
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
        return thoiGianKetThuc.HasValue && thoiGianKetThuc.Value > DateTime.UtcNow;
    }

    private async Task<bool> KhongTrungNgayTao(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return !await _context.BaiTaps.AnyAsync(bt =>
            bt.LichHocId == command.CreateBaiTapDto.LichHocId &&
            bt.NgayTao == today, cancellationToken);
    }

    private async Task<bool> NgayTaoValidWithLichHoc(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var dto = command.CreateBaiTapDto;
        var ngayTao = DateOnly.FromDateTime(DateTime.UtcNow);

        var lichHoc = await _context.LichHocs
            .AsNoTracking()
            .FirstOrDefaultAsync(lh => lh.Id == dto.LichHocId, cancellationToken);

        if (lichHoc == null) return false;

        var thuCuaNgayTao = ConvertDayOfWeekToThu(DateTime.UtcNow.DayOfWeek);

        return
            ngayTao >= lichHoc.NgayBatDau &&
            ngayTao <= lichHoc.NgayKetThuc &&
            thuCuaNgayTao == lichHoc.Thu;
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

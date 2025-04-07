using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
using StudyFlow.Application.Common.Interfaces;

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
            .MustAsync(NgayTaoValidWithLichHoc)
            .WithMessage("Hôm nay không có lịch học của lớp hoặc Bạn không thể tạo bài tập trong buổi dạy thay.");

        RuleFor(x => x)
            .MustAsync(KhongTrungNgayTao)
            .WithMessage("Đã tồn tại bài tập được tạo hôm nay cho lớp học này.");
    }

    private bool ValidTaiLieu(IFormFile file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        bool isValidExtension = allowedExtensions.Contains(extension);
        bool isValidSize = file.Length <= 10 * 1024 * 1024; // <= 10MB

        return isValidExtension && isValidSize;
    }

    private bool BeFutureTime(DateTime? dateTime)
    {
        return dateTime.HasValue && dateTime.Value > DateTime.Now;
    }

    private async Task<bool> NgayTaoValidWithLichHoc(CreateBaiTapCommand command, CancellationToken cancellationToken)
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

        if (lichHoc == null) return false;

        // Không cho phép tạo nếu là lịch học dạy thay
        if (lichHoc.TrangThai != null && lichHoc.TrangThai.Trim().Equals("Dạy thay", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }


    private async Task<bool> KhongTrungNgayTao(CreateBaiTapCommand command, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var dto = command.CreateBaiTapDto;
        var today = DateOnly.FromDateTime(DateTime.Now);
        var coSoId = _identityService.GetCampusId(token);
        var trung = await _context.BaiTaps.AnyAsync(bt =>
            bt.LichHoc.TenLop == dto.TenLop &&
            bt.LichHoc.Phong.CoSoId == coSoId &&
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

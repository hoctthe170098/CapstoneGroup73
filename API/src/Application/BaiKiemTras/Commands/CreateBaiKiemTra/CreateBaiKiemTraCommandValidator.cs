using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateBaiKiemTra;
public class UpdateBaiKiemTraCommandValidator : AbstractValidator<CreateBaiKiemTraCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateBaiKiemTraCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.BaiKiemTraDto.TenBaiKiemTra)
            .MaximumLength(50).WithMessage("Tên bài kiểm tra tối đa 50 ký tự.")
            .MinimumLength(5).WithMessage("Tên bài kiểm tra tối thiểu 5 ký tự.");
        RuleFor(x => x.BaiKiemTraDto.TenLop)
            .MustAsync(ValidLop)
            .WithMessage("Lớp không tồn tại hoặc không còn hoạt động.")
            .MustAsync(ValidLopDaHocDuSoBuoi)
            .WithMessage("Lớp phải học ít nhất 4 buổi mới có thể làm bài kiểm tra");
        RuleFor(x => x.BaiKiemTraDto.NgayKiemTra)
            .MustAsync(ValidNgayKiemTra)
            .WithMessage("Ngày kiểm tra phải sau hôm nay ít nhất 7 ngày, " +
            "phải vào thứ mà học sinh học và phải nhỏ hơn ngày kết thúc khoá học.")
            .MustAsync(ValidLichKiemTraCoDinh)
            .WithMessage("Ngày kiểm tra phải vào lịch học cố định và giáo viên chính.")
            .MustAsync(NotDuplicatedLichThi)
            .WithMessage("Lớp học đã có bài kiểm tra trong ngày này.");
        RuleFor(x => x.BaiKiemTraDto.TaiLieu)
            .Must(ValidTaiLieu)
            .WithMessage("Đề thi phải là word hoặc pdf và độ lớn không quá 10Mb.");
    }

    private async Task<bool> ValidLopDaHocDuSoBuoi(string tenLop, CancellationToken token)
    {
        var soBuoi = await _context.DiemDanhs
            .Where(dd=>dd.ThamGiaLopHoc.LichHoc.TenLop == tenLop)
            .Select(dd=>dd.Ngay)
            .Distinct()
            .ToListAsync();
        if(soBuoi.Count<4) return false;
        return true;
    }

    private async Task<bool> NotDuplicatedLichThi(CreateBaiKiemTraCommand command,
        DateOnly ngayKiemTra, CancellationToken token)
    {
        return !await _context.BaiKiemTras.AnyAsync(b => b.NgayKiemTra == ngayKiemTra
        && b.LichHoc.TenLop == command.BaiKiemTraDto.TenLop);
    }

    private async Task<bool> ValidLichKiemTraCoDinh(CreateBaiKiemTraCommand command
        , DateOnly ngayKiemTra, CancellationToken token)
    {
        var thu = ((int)ngayKiemTra.DayOfWeek > 0) 
            ? (int)ngayKiemTra.DayOfWeek + 1 
            : (int)ngayKiemTra.DayOfWeek + 8;
        return !await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.BaiKiemTraDto.TenLop
        && lh.Thu == thu && lh.NgayKetThuc == ngayKiemTra 
        && (lh.TrangThai == "Dạy bù"||lh.TrangThai=="Dạy thay"));
    }

    private bool ValidTaiLieu(IFormFile file)
    {
        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (fileExtension != ".doc" && fileExtension != ".docx" && fileExtension != ".pdf")
        {
            return false; // File không phải word hoặc pdf
        }

        if (file.Length > 10 * 1024 * 1024) // 10MB
        {
            return false; // File vượt quá 10MB
        }

        return true; // File hợp lệ
    }

    private async Task<bool> ValidNgayKiemTra(CreateBaiKiemTraCommand command,DateOnly ngayKiemTra, CancellationToken token)
    {
        if(ngayKiemTra< DateOnly.FromDateTime(DateTime.Now).AddDays(7)) return false;
        var thu = ((int)ngayKiemTra.DayOfWeek >0) 
            ? (int)ngayKiemTra.DayOfWeek + 1 
            : (int)ngayKiemTra.DayOfWeek + 8;
        return await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.BaiKiemTraDto.TenLop
        && lh.Thu == thu&&lh.NgayKetThuc>=ngayKiemTra&&lh.TrangThai=="Cố định");
    }
    private async Task<bool> ValidLop(string tenLop, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.LichHocs
            .AnyAsync(lh=>lh.Phong.CoSoId == coSoId
            &&lh.TenLop==tenLop
            &&lh.NgayKetThuc> DateOnly.FromDateTime(DateTime.Now));
    }
}

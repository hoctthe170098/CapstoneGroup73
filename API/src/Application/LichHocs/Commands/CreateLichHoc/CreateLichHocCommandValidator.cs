using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;

public class CreateLichHocCommandValidator : AbstractValidator<CreateLichHocCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public CreateLichHocCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.LopHocDto.TenLop)
            .NotEmpty().WithMessage("Tên lớp không được để trống")
            .MaximumLength(20).WithMessage("Tên lớp tối đa 20 ký tự")
            .MustAsync(NotExistClassName)
            .WithMessage("Tên lớp đã tồn tại");

        RuleFor(x => x.LopHocDto.NgayBatDau)
            .Must(BeValidDateOnly).WithMessage("Ngày bắt đầu không đúng định dạng DateOnly")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Ngày bắt đầu phải lớn hơn ngày hiện tại");

        RuleFor(x => x.LopHocDto.NgayKetThuc)
            .Must(BeValidDateOnly).WithMessage("Ngày kết thúc không đúng định dạng DateOnly")
            .GreaterThan(x => x.LopHocDto.NgayBatDau.AddMonths(2))
            .WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu ít nhất 2 tháng");

        RuleFor(x => x.LopHocDto.HocPhi)
            .GreaterThanOrEqualTo(50000).WithMessage("Học phí phải lớn hơn 50,000");

        RuleFor(x => x.LopHocDto.GiaoVienCode)
            .NotEmpty().WithMessage("GiaoVienCode không được để trống")
            .MustAsync(ExistGiaoVien)
            .WithMessage("Giáo viên không tồn tại");

        RuleFor(x => x.LopHocDto.ChuongTrinhId)
            .GreaterThan(0).WithMessage("ChuongTrinhId phải là số nguyên dương")
            .MustAsync(ExistChuongTrinh)
            .WithMessage("Chương trình không tồn tại");

        RuleFor(x => x.LopHocDto.LichHocs)
            .NotEmpty().WithMessage("Danh sách Lịch Học không được để trống")
            .MustAsync(BeValidLichHocs)
            .WithMessage("Danh sách Lịch Học đã bị trùng hoặc không hợp lệ, " +
            "hãy kiểm tra lại lịch của các lớp khác bao gồm cả lịch dạy thay và dạy thêm" +
            "(lưu ý: thời gian nghỉ giữa 2 tiết của 1 giáo viên ít nhất là 15p)");
    }

    private async Task<bool> NotExistClassName(string name, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return !await _context.LichHocs.AnyAsync(l=>l.TenLop==name&&l.Phong.CoSoId==coSoId,cToken);
    }

    private async Task<bool> ExistChuongTrinh(int chuongTrinhId, CancellationToken token)
    {
        return await _context.ChuongTrinhs
            .AnyAsync(ct => ct.Id == chuongTrinhId && ct.TrangThai.ToLower()=="use");
    }

    private async Task<bool> ExistGiaoVien(string code, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.GiaoViens.AnyAsync(gv => gv.Code == code && gv.CoSoId == coSoId);
    }

    private async Task<bool> ExistPhong(int phongId, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.Phongs.AnyAsync(p => p.Id == phongId&&p.CoSoId==coSoId&&p.TrangThai=="use");
    }

    private bool BeValidDateOnly(DateOnly date)
    {
        return date != default; // Kiểm tra xem DateOnly có giá trị mặc định hay không
    }

    private bool BeValidTimeOnly(TimeOnly time)
    {
        return time != default; // Kiểm tra xem TimeOnly có giá trị mặc định hay không
    }

    private async Task<bool> BeValidLichHocs(CreateLichHocCommand command, List<LichHocDto> lichHocs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        if (lichHocs == null || lichHocs.Count == 0)
        {
            return false; // Danh sách rỗng hoặc null thì không cần validate
        }
        var listThu = lichHocs.Select(lh=>lh.Thu).Distinct()
            .ToList();
        if(listThu.Count!=lichHocs.Count) return false;
        foreach (var lichHoc in lichHocs)
        {
            var checktime1 = TimeOnly.TryParse(lichHoc.GioBatDau, out var gioBatDau);
            var checktime2 = TimeOnly.TryParse(lichHoc.GioKetThuc, out var gioKetThuc);
            if(!checktime1||!checktime2) return false;
            if (lichHoc.Thu<2||lichHoc.Thu>8) return false;
            var check = await ExistPhong(lichHoc.PhongId, cToken);
            if (!check) return false;

            // Kiểm tra GioBatDau định dạng TimeOnly
            if (!BeValidTimeOnly(gioBatDau) || gioBatDau < new TimeOnly(8, 0) || gioBatDau > new TimeOnly(20, 0))
            {
                return false;
            }

            // Kiểm tra GioKetThuc định dạng TimeOnly
            if (!BeValidTimeOnly(gioKetThuc) || gioKetThuc < new TimeOnly(10, 0) || gioKetThuc > new TimeOnly(22, 0))
            {
                return false;
            }

            if (gioKetThuc < gioBatDau.AddHours(2))
            {
                return false;
            }
        }
        var lichHocData = await _context.LichHocs
            .Where(l=>l.Phong.CoSoId== coSoId)
            .Select(l => new {l.GiaoVienCode,l.PhongId,l.GioBatDau
            ,l.GioKetThuc,l.Thu,l.NgayBatDau,l.NgayKetThuc})
            .ToListAsync();
        foreach (var lichHoc in lichHocs)
        {
            var checkThuAndPhong = lichHocData
                .Any(lh=>lh.Thu==lichHoc.Thu
                &&(lh.PhongId==lichHoc.PhongId|| lh.GiaoVienCode == command.LopHocDto.GiaoVienCode)
                && TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc,lh.Thu)>=command.LopHocDto.NgayBatDau
                && !(lh.GioKetThuc<= TimeOnly.Parse(lichHoc.GioBatDau).AddMinutes(-15)
                ||lh.GioBatDau>=TimeOnly.Parse(lichHoc.GioKetThuc).AddMinutes(15)));
            if (checkThuAndPhong) return false;
        }
        return true;
    }
    // Phương thức tính toán ngày buổi học cuối cùng
    private DateOnly TinhNgayBuoiHocCuoiCung(DateOnly ngayKetThuc, int thu)
    {
        // Tính toán ngày buổi học cuối cùng dựa trên ngày kết thúc và thứ
        var ngayBuoiHocCuoiCung = ngayKetThuc;
        var ThutrongTuan = (thu < 8) ? thu - 1 : thu - 8;
        while (ngayBuoiHocCuoiCung.DayOfWeek != (DayOfWeek)ThutrongTuan)
        {
            ngayBuoiHocCuoiCung = ngayBuoiHocCuoiCung.AddDays(-1);
        }
        return ngayBuoiHocCuoiCung;
    }
}

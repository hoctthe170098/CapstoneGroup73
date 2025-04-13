using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public class EditLichHocCommandValidator : AbstractValidator<EditLichHocCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public EditLichHocCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.LopHocDto.TenLop)
            .MustAsync(ExistClassName)
            .WithMessage("Lớp đã kết thúc và không thể chỉnh sửa")
            .MustAsync(KhongTrungLichHoc)
            .WithMessage("Lớp có lịch học hôm nay, không thể chỉnh sửa.");
        RuleFor(x => x.LopHocDto.LichHocs)
            .NotEmpty().WithMessage("Danh sách Lịch Học không được để trống")
            .MustAsync(ChuaBatDau)
            .WithMessage("Lớp học đã bắt đầu, không thể update lịch học")
            .MustAsync(ValidLichHoc)
            .WithMessage("Danh sách Lịch Học đã bị trùng hoặc không hợp lệ" +
            "(thời gian nghỉ giữa 2 tiết của 1 giáo viên ít nhất là 15p)");
        RuleFor(x => x.LopHocDto.HocSinhCodes)
            .Must(EnoughHocSinh)
            .WithMessage("Lớp học phải có ít nhất 3 học sinh")
            .MustAsync(ExistHocSinh)
            .WithMessage("Có học sinh không tồn tại trong cơ sở này")
            .Must(ValidLichHocHocSinh)
            .WithMessage("Có học sinh bị trùng lịch, vui lòng kiểm tra lại (bao gồm cả lịch học bù)");
    }

    private async Task<bool> KhongTrungLichHoc(string tenLop, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var HomNay = DateOnly.FromDateTime(DateTime.Now);
        var ThuHomNay = (int)HomNay.DayOfWeek>0?(int)(HomNay.DayOfWeek)+1:8;
        return !await _context.LichHocs
            .AnyAsync(lh => lh.Phong.CoSoId == coSoId && lh.TenLop == tenLop 
            && ((lh.Thu == ThuHomNay&&lh.TrangThai=="Cố định")
            ||(lh.TrangThai=="Dạy bù"&&lh.NgayKetThuc==HomNay)));
    }

    private bool ValidLichHocHocSinh(EditLichHocCommand command
        , List<string> hocSinhCodes)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        List<string> invalidCodes = new List<string>();
        var lichHocMois = command.LopHocDto.LichHocs;
        var lichHocCu = _context.LichHocs
            .Where(lh=>lh.TenLop == command.LopHocDto.TenLop 
            && lh.NgayKetThuc!= DateOnly.MinValue 
            && lh.TrangThai!="Dạy thay"
            && lh.Phong.CoSoId == coSoId)
            .Select(lh=>lh.Id)
            .ToList();
        foreach(var hs in command.LopHocDto.HocSinhCodes )
        {
            var lichHocHocSinhHienTai = _context.ThamGiaLopHocs
                .Where(tg => tg.HocSinhCode == hs 
                && !lichHocCu.Contains(tg.LichHoc.Id) 
                && tg.LichHoc.Phong.CoSoId == coSoId)
                .Select(tg => tg.LichHoc)
                .Distinct()
                .ToList();
            foreach(var lichHocMoi in lichHocMois)
            {
                var checkLichSinhVien = lichHocHocSinhHienTai
                    .Any(lh => lh.Thu == lichHocMoi.Thu
                    && !(lh.GioKetThuc <= TimeOnly.Parse(lichHocMoi.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichHocMoi.GioKetThuc).AddMinutes(15)) 
                && TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= command.LopHocDto.NgayBatDau);
                if(checkLichSinhVien)
                {
                    invalidCodes.Add(hs);
                    break;
                }
            }
        }
        string tatCaLoi = string.Join(", ", invalidCodes);
        if(invalidCodes.Count>0) throw new Exception($"Học sinh {tatCaLoi} bị trùng lịch");
        return invalidCodes.Count == 0;
    }
    private async Task<bool> ExistHocSinh(List<string> list, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        foreach (var item in list)
        {
            var hocSinh = await _context.HocSinhs
                .FirstOrDefaultAsync(hs => hs.Code == item && hs.CoSoId == coSoId);
            if (hocSinh==null) return false;
            var active = await _identityService.IsUserActiveAsync(hocSinh.UserId!);
            if(!active) return false;
        }
        return true;
    }

    private bool EnoughHocSinh(List<string> list)
    {
        return list.Count >= 1;
    }

    private async Task<bool> ChuaBatDau(EditLichHocCommand command, 
        List<LichHocDto> lichHocs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var daBatDau = await _context.LichHocs
                    .AnyAsync(lh => lh.TenLop == command.LopHocDto.TenLop && lh.TrangThai == "Cố định"
                  && lh.NgayBatDau <= DateOnly.FromDateTime(DateTime.Now)&&lh.Phong.CoSoId==coSoId);

        var lichHocHienTais = await _context.LichHocs
                 .Where(lh => lh.TenLop == command.LopHocDto.TenLop && lh.Phong.CoSoId == coSoId)
                 .ToListAsync();
        if (daBatDau)
        {
            foreach (var lh in lichHocs)
            {
                if (lh.Id == null) return false;
                var lichHoc = lichHocHienTais.FirstOrDefault(l => l.Id == lh.Id);
                if(lichHoc == null) return false;
                if(lh.Thu!=lichHoc.Thu
                    ||TimeOnly.Parse(lh.GioBatDau)!=lichHoc.GioBatDau
                    ||TimeOnly.Parse(lh.GioKetThuc)!=lichHoc.GioKetThuc) return false;
            }
        }
        return true;
    }

    private async Task<bool> ValidLichHoc(EditLichHocCommand command, 
        List<LichHocDto> lichHocs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichHocHienTais = await _context.LichHocs
                 .Where(lh => lh.TenLop == command.LopHocDto.TenLop && lh.Phong.CoSoId == coSoId)
                 .Select(lh => lh.Id)
                 .ToListAsync();
            if (lichHocs == null || lichHocs.Count == 0)
            {
                return false; 
            }
            var listThu = lichHocs.Select(lh => lh.Thu).Distinct()
                .ToList();
            if (listThu.Count != lichHocs.Count) return false;
            foreach (var lichHoc in lichHocs)
            {
                var checktime1 = TimeOnly.TryParse(lichHoc.GioBatDau, out var gioBatDau);
                var checktime2 = TimeOnly.TryParse(lichHoc.GioKetThuc, out var gioKetThuc);
                if (!checktime1 || !checktime2) return false;
                if (lichHoc.Thu < 2 || lichHoc.Thu > 8) return false;
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
                .Where(lh=>!lichHocHienTais.Contains(lh.Id)
                && lh.Phong.CoSoId == coSoId
                &&lh.TrangThai!="Dạy thay"
                &&lh.NgayKetThuc!=DateOnly.MinValue)
                .Select(l => new {
                    l.GiaoVienCode,
                    l.PhongId,
                    l.GioBatDau,
                    l.GioKetThuc,
                    l.Thu,
                    l.NgayBatDau,
                    l.NgayKetThuc
                })
                .ToListAsync();
            foreach (var lichHoc in lichHocs)
            {
                var checkThuAndPhong = lichHocData
                    .Any(lh => lh.Thu == lichHoc.Thu
                    && (lh.PhongId == lichHoc.PhongId || lh.GiaoVienCode == command.LopHocDto.GiaoVienCode)
                    && TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= command.LopHocDto.NgayBatDau
                    && !(lh.GioKetThuc <= TimeOnly.Parse(lichHoc.GioBatDau).AddMinutes(-15)
                    || lh.GioBatDau >= TimeOnly.Parse(lichHoc.GioKetThuc).AddMinutes(15)));
                if (checkThuAndPhong) return false;
            }
        return true;
    }
    private bool BeValidTimeOnly(TimeOnly time)
    {
        return time != default; // Kiểm tra xem TimeOnly có giá trị mặc định hay không
    }
    private async Task<bool> ExistPhong(int phongId, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.Phongs.AnyAsync(p => p.Id == phongId && p.CoSoId == coSoId && p.TrangThai == "use");
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
    private async Task<bool> ExistClassName(EditLichHocCommand command, 
        string tenLop, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.LichHocs.AnyAsync(l => l.TenLop == tenLop 
        && l.Phong.CoSoId == coSoId
        && l.NgayBatDau == command.LopHocDto.NgayBatDau
        && l.NgayKetThuc == command.LopHocDto.NgayKetThuc
        && l.GiaoVienCode == command.LopHocDto.GiaoVienCode
        && l.HocPhi == command.LopHocDto.HocPhi
        && l.ChuongTrinhId == command.LopHocDto.ChuongTrinhId
        && l.NgayKetThuc > DateOnly.FromDateTime(DateTime.Now), cToken);
    }
}

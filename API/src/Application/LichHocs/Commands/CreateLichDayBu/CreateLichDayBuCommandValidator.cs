using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.LichHocs.Commands.CreateLichDayBu;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichDayBu;

public class CreateLichDayBuCommandValidator : AbstractValidator<CreateLichDayBuCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public CreateLichDayBuCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.TenLop)
            .MustAsync(ExistClassName)
            .WithMessage("Lớp không tồn tại trong cơ sở này");
        RuleFor(x => x.NgayNghi)
            .Must(SauHomNay)
            .WithMessage("Ngày nghỉ phải sau ngày hôm nay.")
            .MustAsync(SauNgayBatDau)
            .WithMessage("Ngày nghỉ phải sau ngày bắt đầu và trước ngày kết thúc của lớp học.")
            .MustAsync(TrungLichHocCoDinh)
            .WithMessage("Ngày nghỉ phải trùng với lịch học cố định của lớp.")
            .MustAsync(ChuaDuocNghi)
            .WithMessage("Lớp đã được nghỉ vào ngày này")
            .MustAsync(KhongTrungNgayKiemTra)
            .WithMessage("Lớp có bài kiểm tra vào ngày này, không thể nghỉ.");
        RuleFor(x => x.LichDayBu)
            .MustAsync(ValidLichHocBu)
            .WithMessage("Lịch học bù không hợp lệ, ngày học bù phải sau ngày hôm này" +
            ", giờ bắt đầu phải sau 8h sáng và giờ kết thúc phải trước 10h tối" +
            ", mỗi tiết học kéo dài ít nhất 2 tiếng")
            .MustAsync(KhongTrungLichDayCoDinh)
            .WithMessage("Lịch học này đã bị trùng với lịch dạy cố định")
            .MustAsync(KhongTrungLichDayGiaoVien)
            .WithMessage("Lịch học này đã bị trùng với lịch giáo viên.")
            .MustAsync(KhongTrungPhong)
            .WithMessage("Lịch học này đã bị trùng với phòng học khác")
            .MustAsync(KhongTrungLichVoiHocSinh)
            .WithMessage("Lịch học này đã bị trùng với lịch của 1 học sinh nào đó trong lớp");
    }

    private async Task<bool> KhongTrungLichVoiHocSinh(CreateLichDayBuCommand command, 
        LichDayBuDto? lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return true;
        var thu = ((int)lichDayBu.NgayHocBu.DayOfWeek > 0)
    ? (int)lichDayBu.NgayHocBu.DayOfWeek + 1
    : (int)lichDayBu.NgayHocBu.DayOfWeek + 8;
        var lichCoDinh =  _context.LichHocs
            .Where(lh=>lh.TenLop==command.TenLop && lh.TrangThai=="Cố định")
            .Select(lh=>lh.Id)
            .ToList();
        var hocSinhCodes = _context.ThamGiaLopHocs
            .Where(tg=>lichCoDinh.Contains(tg.LichHocId))
            .Select(tg=>tg.HocSinhCode)
            .Distinct()
            .ToList();
        foreach(var code in hocSinhCodes)
        {
            var lichHocSinhVien = await _context.LichHocs
                .Where(lh => lh.ThamGiaLopHocs.Select(tg => tg.HocSinhCode).Contains(code)
                && lh.NgayKetThuc != DateOnly.MinValue
                && lh.Thu == thu
                && !(lh.GioKetThuc <= TimeOnly.Parse(lichDayBu.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichDayBu.GioKetThuc).AddMinutes(15))).ToListAsync();
            var checkLichSinhVien = lichHocSinhVien
                .Any(lh => (TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= lichDayBu.NgayHocBu 
                            && lh.TrangThai == "Cố định"
                            && ngayNghi(lh.Id) != lichDayBu.NgayHocBu)
                          || ((lh.TrangThai == "Dạy bù") && lh.NgayKetThuc == lichDayBu.NgayHocBu));
            if (checkLichSinhVien) return false;
        }
        return true;
    }
    private DateOnly? ngayNghi(Guid lichHocId)
    {
        var ngayNghi = _context.LichHocs
            .Where(lh => lh.LichHocGocId == lichHocId && lh.NgayKetThuc == DateOnly.MinValue
            && lh.TrangThai == "Học bù")
            .Select(lh => lh.NgayHocGoc)
            .FirstOrDefault();
        return ngayNghi;
    }
    private async Task<bool> KhongTrungLichDayCoDinh(CreateLichDayBuCommand command, 
        LichDayBuDto? lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return true;
        var thu = ((int)lichDayBu.NgayHocBu.DayOfWeek > 0)
    ? (int)lichDayBu.NgayHocBu.DayOfWeek + 1
    : (int)lichDayBu.NgayHocBu.DayOfWeek + 8;
        var lichCoDinh = await _context.LichHocs
            .Where(lh => lh.TenLop == command.TenLop && lh.TrangThai == "Cố định"
            && lh.Thu == thu
            && !(lh.GioKetThuc <= TimeOnly.Parse(lichDayBu.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichDayBu.GioKetThuc).AddMinutes(15))).ToListAsync();
        var checkLichHocCoDinh = lichCoDinh
            .Any(lh =>TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= lichDayBu.NgayHocBu);
        if (checkLichHocCoDinh) return false;
        return true;
    }

    private async Task<bool> ValidLichHocBu(LichDayBuDto? lichDayBu, CancellationToken cToken)
    {
        if (lichDayBu == null) return true;
        if(lichDayBu.NgayHocBu < DateOnly.FromDateTime(DateTime.Now)) return false;
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var coSo = await _context.Phongs
            .AnyAsync(p =>p.Id==lichDayBu.PhongId && p.CoSoId == coSoId && p.TrangThai == "use");
        if(!coSo) return false;
        var checkGioBatDau = TimeOnly.TryParse(lichDayBu.GioBatDau, out var gioBatDau);
        var checkGioKetThuc = TimeOnly.TryParse(lichDayBu.GioKetThuc, out var gioKetThuc);
        if(!checkGioBatDau||!checkGioKetThuc) return false;
        // Kiểm tra GioBatDau định dạng TimeOnly
        if ( gioBatDau < new TimeOnly(8, 0) || gioBatDau > new TimeOnly(20, 0))
        {
            return false;
        }

        // Kiểm tra GioKetThuc định dạng TimeOnly
        if ( gioKetThuc < new TimeOnly(10, 0) || gioKetThuc > new TimeOnly(22, 0))
        {
            return false;
        }

        if (gioKetThuc < gioBatDau.AddHours(2))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> KhongTrungPhong(CreateLichDayBuCommand command, 
        LichDayBuDto? lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return true;
        var thu = ((int)lichDayBu.NgayHocBu.DayOfWeek > 0)
    ? (int)lichDayBu.NgayHocBu.DayOfWeek + 1
    : (int)lichDayBu.NgayHocBu.DayOfWeek + 8;
        var lichHocPhong = await _context.LichHocs
            .Where(lh => lh.TenLop != command.TenLop && ngayNghi(lh.Id) != lichDayBu.NgayHocBu
            && lh.Thu == thu
            && lh.PhongId == lichDayBu.PhongId
            && !(lh.GioKetThuc <= TimeOnly.Parse(lichDayBu.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichDayBu.GioKetThuc).AddMinutes(15)))
            .ToListAsync();
        var checkLichHocPhong = lichHocPhong
            .Any(lh => (TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= lichDayBu.NgayHocBu && lh.TrangThai == "Cố định")
            || ((lh.TrangThai == "Dạy bù") && lh.NgayKetThuc == lichDayBu.NgayHocBu));
        if (checkLichHocPhong) return false;
        return true;
    }

    private async Task<bool> KhongTrungLichDayGiaoVien(CreateLichDayBuCommand command,
        LichDayBuDto? lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return true;
        var thu = ((int)lichDayBu.NgayHocBu.DayOfWeek > 0)
    ? (int)lichDayBu.NgayHocBu.DayOfWeek + 1
    : (int)lichDayBu.NgayHocBu.DayOfWeek + 8;
        var giaoVienCode = _context.LichHocs
            .Where(lh=>lh.TenLop==command.TenLop && lh.TrangThai=="Cố định")
            .Select(lh=>lh.GiaoVienCode)
            .FirstOrDefault();
        if(giaoVienCode==null) return false;
        var lichDayGiaoVien = await _context.LichHocs
            .Where(lh => lh.TenLop != command.TenLop && ngayNghi(lh.Id) != lichDayBu.NgayHocBu
            && lh.NgayKetThuc != DateOnly.MinValue
            && lh.Thu == thu
            && lh.GiaoVienCode == giaoVienCode
            && !(lh.GioKetThuc <= TimeOnly.Parse(lichDayBu.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichDayBu.GioKetThuc).AddMinutes(15)))
            .ToListAsync();
        var checkLichDayGiaoVien = lichDayGiaoVien
            .Any(lh => (TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= lichDayBu.NgayHocBu && lh.TrangThai == "Cố định")
            || ((lh.TrangThai == "Dạy bù" || lh.TrangThai == "Dạy thay") && lh.NgayKetThuc == lichDayBu.NgayHocBu));
        if (checkLichDayGiaoVien) return false;
        return true;
    }

    private bool SauHomNay(DateOnly ngayDay)
    {
       if(ngayDay <= DateOnly.FromDateTime(DateTime.Now)) return false;
        return true;
    }
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

    private async Task<bool> KhongTrungNgayKiemTra(CreateLichDayBuCommand command,
        DateOnly ngayDay,CancellationToken token)
    {
        return ! await _context.BaiKiemTras
            .AnyAsync(b => b.NgayKiemTra == ngayDay && b.LichHoc.TenLop == command.TenLop);
    }
    private async Task<bool> ChuaDuocNghi(CreateLichDayBuCommand command, 
        DateOnly ngayDay, CancellationToken token)
    {
        if(command.LichDayBu==null) return !await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.TenLop
      && lh.NgayHocGoc == ngayDay
      && lh.TrangThai == "Dạy bù");
        else return !await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.TenLop
      && lh.NgayHocGoc == ngayDay
      && lh.TrangThai == "Dạy bù"
      &&lh.NgayKetThuc!=DateOnly.MinValue);
    }
    private async Task<bool> SauNgayBatDau(CreateLichDayBuCommand command, 
        DateOnly ngayDay,CancellationToken token)
    {
        var lichHoc =  await _context.LichHocs
           .FirstOrDefaultAsync(lh => lh.TenLop == command.TenLop && lh.TrangThai == "Cố định");
        if (lichHoc == null) return false;
        if (lichHoc.NgayBatDau > ngayDay || lichHoc.NgayKetThuc < ngayDay) return false;
        return true;
    }
    private async Task<bool> TrungLichHocCoDinh(CreateLichDayBuCommand command, 
        DateOnly ngayDay,CancellationToken token)
    {
        var thu = ((int)ngayDay.DayOfWeek > 0)
            ? (int)ngayDay.DayOfWeek + 1
            : (int)ngayDay.DayOfWeek + 8;
        return await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.TenLop
        && lh.Thu == thu && lh.TrangThai == "Cố định");
    }
    private async Task<bool> ExistClassName(string name,CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId =  _identityService.GetCampusId(token);
        return await _context.LichHocs.AnyAsync(l => l.TenLop == name && l.Phong.CoSoId == coSoId);
    }
}

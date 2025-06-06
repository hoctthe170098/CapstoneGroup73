﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.LichHocs.Commands.CreateLichDayBu;
using StudyFlow.Application.LichHocs.Commands.UpdateLichDayBu;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StudyFlow.Application.LichHocs.Commands.UpdateLichDayBu;

public class UpdateLichDayBuCommandValidator : AbstractValidator<UpdateLichDayBuCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateLichDayBuCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.Id)
            .MustAsync(ExistLichDayBu)
            .WithMessage("Lịch dạy bù không tồn tại")
            .MustAsync(ChuaHocBu)
            .WithMessage("Lịch học này đã hết hạn chỉnh sửa");
        RuleFor(x => x.LichDayBu)
            .MustAsync(ValidLichHocBu)
            .WithMessage("Lịch học bù không hợp lệ, ngày học bù phải sau ngày hôm nay và trước ngày kết thúc lớp học" +
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

    private async Task<bool> ChuaHocBu(UpdateLichDayBuCommand command, 
        Guid id, CancellationToken token)
    {
        var lichHocBu = await _context.LichHocs.FirstOrDefaultAsync(lh => lh.Id == id
        && lh.TenLop == command.TenLop
        && lh.NgayHocGoc == command.NgayNghi
        && lh.NgayKetThuc != DateOnly.MinValue);
        if(lichHocBu == null) return false;
        if(lichHocBu.NgayKetThuc <= DateOnly.FromDateTime(DateTime.Now)) return false;
        return true;
    }

    private async Task<bool> ExistLichDayBu(UpdateLichDayBuCommand command, 
        Guid id, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.LichHocs
            .AnyAsync(lh=>lh.Id == id
            && lh.Phong.CoSoId == coSoId
        &&lh.TenLop==command.TenLop
        &&lh.NgayHocGoc==command.NgayNghi
        && lh.NgayKetThuc!=DateOnly.MinValue);
    }

    private async Task<bool> KhongTrungLichVoiHocSinh(UpdateLichDayBuCommand command, 
        LichDayBuDto lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return false;
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
                && lh.Id != command.Id
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
            && lh.TrangThai == "Dạy bù")
            .Select(lh => lh.NgayHocGoc)
            .FirstOrDefault();
        return ngayNghi;
    }
    private async Task<bool> KhongTrungLichDayCoDinh(UpdateLichDayBuCommand command, 
        LichDayBuDto lichDayBu, CancellationToken token)
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

    private async Task<bool> ValidLichHocBu(UpdateLichDayBuCommand command, LichDayBuDto lichDayBu, CancellationToken cToken)
    {
        if (lichDayBu == null) return true;
        if(lichDayBu.NgayHocBu <= DateOnly.FromDateTime(DateTime.Now)) return false;
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichHoc = await _context.LichHocs.FirstOrDefaultAsync(lh => lh.TenLop == command.TenLop && lh.Phong.CoSoId == coSoId&&lh.TrangThai=="Cố định");
        if (lichHoc == null) return false;
        if (lichHoc.NgayKetThuc < lichDayBu.NgayHocBu || lichHoc.NgayBatDau > lichDayBu.NgayHocBu) return false;
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

    private async Task<bool> KhongTrungPhong(UpdateLichDayBuCommand command, 
        LichDayBuDto lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return true;
        var thu = ((int)lichDayBu.NgayHocBu.DayOfWeek > 0)
    ? (int)lichDayBu.NgayHocBu.DayOfWeek + 1
    : (int)lichDayBu.NgayHocBu.DayOfWeek + 8;
        var lichHocPhong = await _context.LichHocs
            .Where(lh => lh.TenLop != command.TenLop && lh.Id != command.Id
            && lh.Thu == thu
            && lh.PhongId == lichDayBu.PhongId
            && !(lh.GioKetThuc <= TimeOnly.Parse(lichDayBu.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichDayBu.GioKetThuc).AddMinutes(15)))
            .ToListAsync();
        var checkLichHocPhong = lichHocPhong
            .Any(lh =>ngayNghi(lh.Id) != lichDayBu.NgayHocBu
            && (TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= lichDayBu.NgayHocBu && lh.TrangThai == "Cố định")
            || ((lh.TrangThai == "Dạy bù") && lh.NgayKetThuc == lichDayBu.NgayHocBu));
        if (checkLichHocPhong) return false;
        return true;
    }

    private async Task<bool> KhongTrungLichDayGiaoVien(UpdateLichDayBuCommand command,
        LichDayBuDto lichDayBu, CancellationToken token)
    {
        if (lichDayBu == null) return true;
        var thu = ((int)lichDayBu.NgayHocBu.DayOfWeek > 0)
    ? (int)lichDayBu.NgayHocBu.DayOfWeek + 1
    : (int)lichDayBu.NgayHocBu.DayOfWeek + 8;
        var giaoVienCode = _context.LichHocs
            .Where(lh=>lh.TenLop==command.TenLop && lh.TrangThai=="Cố định" && lh.Id != command.Id)
            .Select(lh=>lh.GiaoVienCode)
            .FirstOrDefault();
        if(giaoVienCode==null) return false;
        var lichDayGiaoVien = await _context.LichHocs
            .Where(lh => lh.TenLop != command.TenLop 
            && lh.NgayKetThuc != DateOnly.MinValue
            && lh.Thu == thu
            && lh.GiaoVienCode == giaoVienCode
            && !(lh.GioKetThuc <= TimeOnly.Parse(lichDayBu.GioBatDau).AddMinutes(-15)
                || lh.GioBatDau >= TimeOnly.Parse(lichDayBu.GioKetThuc).AddMinutes(15)))
            .ToListAsync();
        var checkLichDayGiaoVien = lichDayGiaoVien
            .Any(lh => ngayNghi(lh.Id) != lichDayBu.NgayHocBu
            && (TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= lichDayBu.NgayHocBu && lh.TrangThai == "Cố định")
            || ((lh.TrangThai == "Dạy bù" || lh.TrangThai == "Dạy thay") && lh.NgayKetThuc == lichDayBu.NgayHocBu));
        if (checkLichDayGiaoVien) return false;
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
}

﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.LichHocs.Commands.CreateLichDayThay;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace StudyFlow.Application.LichHocs.Commands.UpdateLichDayThay;

public class UpdateLichDayThayCommandValidator : AbstractValidator<UpdateLichDayThayCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateLichDayThayCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.Id)
            .MustAsync(ExistLichDayThay)
            .WithMessage("Lịch dạy này không thể chỉnh sửa.");
        RuleFor(x => x.TenLop)
            .MustAsync(ExistClassName)
            .WithMessage("Lớp không tồn tại trong cơ sở này");
        RuleFor(x => x.GiaoVienCode)
            .MustAsync(ExistGiaoVien)
            .WithMessage("Giáo viên không tồn tại trong cơ sở này.")
            .MustAsync(NotDuplicatedGiaoVien)
            .WithMessage("Giáo viên dạy thay không được trùng với giáo viên cố định.");
        RuleFor(x => x.NgayDay)
            .Must(SauHomNay)
            .WithMessage("Ngày dạy phải sau ngày hôm nay.")
            .MustAsync(SauNgayBatDau)
            .WithMessage("Ngày dạy phải sau ngày bắt đầu và trước ngày kết thúc của lớp học.")
            .MustAsync(TrungLichHocCoDinh)
            .WithMessage("Ngày dạy phải trùng với lịch học cố định của lớp.")
            .MustAsync(ChuaCoLichDayThayNao)
            .WithMessage("Lớp đã có lịch dạy thay vào ngày này.")
            .MustAsync(KhongTrungNgayKiemTra)
            .WithMessage("Lớp có bài kiểm tra vào ngày này, không thể đổi giáo viên dạy thay.")
            .MustAsync(KhongTrungNgayNghi)
            .WithMessage("Lớp đã được nghỉ vào ngày này")
            .MustAsync(KhongTrungLichGiaoVien)
            .WithMessage("Giáo viên dạy thay bị trùng lịch" +
            "(lưu ý: giữa 2 slot của lớp phải có ít nhất 15 phút nghỉ).");
    }
    private async Task<bool> KhongTrungNgayNghi(UpdateLichDayThayCommand command,
        DateOnly ngayDay, CancellationToken token)
    {
        return !await _context.LichHocs.AnyAsync(lh => lh.TrangThai == "Dạy bù"
        && lh.TenLop == command.TenLop && lh.NgayHocGoc == ngayDay);
    }
    private bool SauHomNay(DateOnly ngayDay)
    {
        if (ngayDay <= DateOnly.FromDateTime(DateTime.Now)) return false;
        return true;
    }

    private async Task<bool> ExistLichDayThay(UpdateLichDayThayCommand command, 
        Guid id, CancellationToken token)
    {
        var lichHoc = await _context.LichHocs
            .FirstOrDefaultAsync(lh => lh.Id == id 
            && lh.TenLop == command.TenLop
            && lh.TrangThai == "Dạy thay" 
            && lh.NgayKetThuc > DateOnly.FromDateTime(DateTime.Now));
        if(lichHoc == null) return false;
        return true;
    }

    private async Task<bool> KhongTrungLichGiaoVien(UpdateLichDayThayCommand command
        ,DateOnly ngayDay,CancellationToken token)
    {
        var thu = ((int)ngayDay.DayOfWeek > 0)
            ? (int)ngayDay.DayOfWeek + 1
            : (int)ngayDay.DayOfWeek + 8;
        var lichDinhDoi = await _context.LichHocs
            .FirstOrDefaultAsync(lh=>lh.Thu == thu&&lh.TenLop == command.TenLop);
        if (lichDinhDoi == null) return false;
        var lichGiaoVienDayThay =  _context.LichHocs
            .Where(lh=>lh.Id!=command.Id&&lh.GiaoVienCode==command.GiaoVienCode)
            .ToList();
        var checkLichGiaoVien = lichGiaoVienDayThay
            .Any(lh => lh.Thu == lichDinhDoi.Thu && ngayNghi(lh.Id) != command.NgayDay
            && ((TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= command.NgayDay
                && lh.TrangThai == "Cố định")
               || ((lh.TrangThai == "Dạy bù" || lh.TrangThai == "Dạy thay") && lh.NgayKetThuc == command.NgayDay))
            && !(lh.GioKetThuc <= lichDinhDoi.GioBatDau.AddMinutes(-15)
            || lh.GioBatDau >= lichDinhDoi.GioKetThuc.AddMinutes(15)));
        if (checkLichGiaoVien) return false;
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

    private async Task<bool> KhongTrungNgayKiemTra(UpdateLichDayThayCommand command,
        DateOnly ngayDay,CancellationToken token)
    {
        return ! await _context.BaiKiemTras
            .AnyAsync(b => b.NgayKiemTra == ngayDay && b.LichHoc.TenLop == command.TenLop);
    }
    private async Task<bool> ChuaCoLichDayThayNao(UpdateLichDayThayCommand command, 
        DateOnly ngayDay, CancellationToken token)
    {
        return ! await _context.LichHocs
            .AnyAsync(lh =>lh.Id != command.Id 
                        && lh.TenLop == command.TenLop
                        && lh.NgayHocGoc == ngayDay
                        && lh.TrangThai == "Dạy thay");
    }
    private async Task<bool> SauNgayBatDau(UpdateLichDayThayCommand command, 
        DateOnly ngayDay,CancellationToken token)
    {
        var lichHoc =  await _context.LichHocs
           .FirstOrDefaultAsync(lh => lh.TenLop == command.TenLop && lh.TrangThai == "Cố định");
        if (lichHoc == null) return false;
        if (lichHoc.NgayBatDau > ngayDay || lichHoc.NgayKetThuc < ngayDay) return false;
        return true;
    }
    private async Task<bool> TrungLichHocCoDinh(UpdateLichDayThayCommand command, 
        DateOnly ngayDay,CancellationToken token)
    {
        var thu = ((int)ngayDay.DayOfWeek > 0)
            ? (int)ngayDay.DayOfWeek + 1
            : (int)ngayDay.DayOfWeek + 8;
        return await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.TenLop
        && lh.Thu == thu && lh.TrangThai == "Cố định");
    }
    private async Task<bool> NotDuplicatedGiaoVien(UpdateLichDayThayCommand command
        ,string giaoVienDayThayCode,CancellationToken token)
    {
        return !await _context.LichHocs
            .AnyAsync(lh => lh.TenLop == command.TenLop
            && lh.GiaoVienCode == command.GiaoVienCode
            && lh.TrangThai == "Cố định");
    }
    private async Task<bool> ExistClassName(string name,CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId =  _identityService.GetCampusId(token);
        return await _context.LichHocs.AnyAsync(l => l.TenLop == name && l.Phong.CoSoId == coSoId);
    }

    private async Task<bool> ExistGiaoVien(string code,CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.GiaoViens.AnyAsync(gv => gv.Code == code && gv.CoSoId == coSoId);
    }
}

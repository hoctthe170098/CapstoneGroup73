﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay;

public class UpdateDiemDanhTheoNgayCommandValidator : AbstractValidator<UpdateDiemDanhTheoNgayCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateDiemDanhTheoNgayCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.UpdateDiemDanhs)
            .MustAsync(TonTaiDiemDanh)
            .WithMessage("Không tồn tại điểm danh này")
            .MustAsync(CungChungMotLopVaNgay)
            .WithMessage("Các điểm danh phải cùng chung 1 lớp và ngày")
            .MustAsync(DuocQuyenChinhSua)
            .WithMessage("Bạn không thể chỉnh sửa điểm danh này")
            .MustAsync(ChuaHetHan)
            .WithMessage("Điểm danh này đã hết hạn chỉnh sửa")
            .Must(DungFormatDiemDanh)
            .WithMessage("Điểm danh có format chưa đúng");
    }

    private bool DungFormatDiemDanh(List<UpdateDiemDanhDto> list)
    {
        foreach (var dto in list)
        {
            if(dto.TrangThai!="Vắng"&&dto.TrangThai!="Có mặt") return false;
            if(dto.DiemBTVN<0||dto.DiemBTVN>10) return false;
            if(dto.DiemTrenLop<0||dto.DiemTrenLop>10) return false;
            if(dto.NhanXet!.Length>200) return false;
        }
        return true;
    }

    private async Task<bool> ChuaHetHan(List<UpdateDiemDanhDto> diemDanhs, CancellationToken token)
    {
        var idDiemDanh = diemDanhs.Select(d => d.Id).ToList();
        foreach(var item in idDiemDanh)
        {
            var diemDanh = await _context.DiemDanhs
                .Include(d=>d.ThamGiaLopHoc.LichHoc)
                .ThenInclude(lh=>lh.LichHocGoc)
                .FirstOrDefaultAsync(d => d.Id == item);
            if(diemDanh == null) return false;
            if(diemDanh.Ngay!=DateOnly.FromDateTime(DateTime.Now)) return false;
            var lichHoc = diemDanh.ThamGiaLopHoc.LichHoc;
            if (lichHoc.TrangThai == "Dạy bù") lichHoc = lichHoc.LichHocGoc;
            if (lichHoc.NgayKetThuc == DateOnly.FromDateTime(DateTime.Now))
            {
                if (lichHoc.GioKetThuc < TimeOnly.FromDateTime(DateTime.Now)) return false;
            }
        }
        return true;
    }

    private async Task<bool> CungChungMotLopVaNgay(List<UpdateDiemDanhDto> diemDanhs, CancellationToken token)
    {
        var diemdanh = await _context.DiemDanhs
            .Where(d=>diemDanhs.Select(dd=>dd.Id).ToList().Contains(d.Id))
            .Include(d=>d.ThamGiaLopHoc)
            .ToListAsync();
        var lichHoc = diemdanh.Select(d => d.ThamGiaLopHoc.LichHocId).Distinct();
        var ngay = diemdanh.Select(d=>d.Ngay).Distinct();
        return (lichHoc.Count() == 1 && ngay.Count() == 1);
    }

    private async Task<bool> TonTaiDiemDanh(List<UpdateDiemDanhDto> diemDanhs, CancellationToken token)
    {
        foreach (var item in diemDanhs)
        {
            var check = await _context.DiemDanhs.AnyAsync(d => d.Id == item.Id);
            if(!check) return false;
        }
        return true;
    }

    private async Task<bool> DuocQuyenChinhSua(List<UpdateDiemDanhDto> diemDanhs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var UserId = _identityService.GetUserId(token);
        var coSoId = _identityService.GetCampusId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == UserId.ToString());
        if (giaoVien == null) throw new NotFoundIDException();
        var idDiemDanh = diemDanhs.Select(d => d.Id).ToList();
        foreach(var id in idDiemDanh)
        {
            var diemDanh = await _context.DiemDanhs
            .Include(d=>d.ThamGiaLopHoc.LichHoc)
            .FirstOrDefaultAsync(dd => dd.Id == id
            && (dd.ThamGiaLopHoc.LichHoc.GiaoVienCode == giaoVien.Code
            || _context.LichHocs.Any(lh => lh.GiaoVienCode == giaoVien.Code && lh.LichHocGocId == dd.ThamGiaLopHoc.LichHocId)));
            if (diemDanh == null) return false;
            if (diemDanh.ThamGiaLopHoc.LichHoc.TrangThai == "Dạy thay")
            {
                var lichHocGoc = _context.LichHocs
                    .First(lh=>lh.Id==diemDanh.ThamGiaLopHoc.LichHoc.LichHocGocId);
                if(lichHocGoc.GiaoVienCode== giaoVien.Code) return false;
            }
        }
        return true;
    }
}

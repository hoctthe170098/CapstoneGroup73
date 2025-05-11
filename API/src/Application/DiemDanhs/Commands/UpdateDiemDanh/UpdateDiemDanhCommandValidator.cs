using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanh;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudyFlow.Application.LichHocs.Commands.UpdateDiemDanh;

public class UpdateDiemDanhCommandValidator : AbstractValidator<UpdateDiemDanhCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateDiemDanhCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.Ngay)
            .MustAsync(TonTaiLichHoc)
            .WithMessage("Không tồn tại lịch học của lớp trong ngày này");
        RuleFor(x => x.UpdateDiemDanhs)
            .MustAsync(TonTaiDiemDanh)
            .WithMessage("Không tồn tại 1 trong những điểm danh này.")
            .MustAsync(DuocQuyenChinhSua)
            .WithMessage("Bạn không thể chỉnh sửa điểm danh này.")
            .MustAsync(ChuaHetHan)
            .WithMessage("Điểm danh này đã hết hạn chỉnh sửa.")
            .Must(DungFormatDiemDanh)
            .WithMessage("Điểm danh có format chưa đúng.");
    }

    private async Task<bool> TonTaiLichHoc(UpdateDiemDanhCommand command, DateOnly ngay, CancellationToken token)
    {
        int thu = (int)(ngay.DayOfWeek);
        if (thu == 0) thu = 8; else thu++;
        return await _context.LichHocs
            .AnyAsync(lh => lh.TenLop == command.TenLop && lh.Thu == thu);
    }

    private bool DungFormatDiemDanh(List<UpdateDiemDanhDto> list)
    {
        foreach (var dto in list)
        {
            if (dto.TrangThai != "Vắng" && dto.TrangThai != "Có mặt" && dto.TrangThai != "Không điểm danh")
                return false;
        }
        return true;
    }

    private async Task<bool> ChuaHetHan(List<UpdateDiemDanhDto> diemDanhs, CancellationToken token)
    {
        var idDiemDanh = diemDanhs.Select(d => d.Id).ToList();
        foreach (var item in idDiemDanh)
        {
            if (item != null)
            {
                var DiemDanh = await _context.DiemDanhs
    .Include(d => d.ThamGiaLopHoc)
    .ThenInclude(tg => tg.LichHoc)
    .ThenInclude(lh => lh.LichHocGoc)
    .FirstOrDefaultAsync(d => d.Id == item);
                if (DiemDanh == null)
                    return false;
                var lichHoc = DiemDanh.ThamGiaLopHoc.LichHoc;
                if (lichHoc == null) return false;
                if (lichHoc.TrangThai == "Dạy bù") lichHoc = lichHoc.LichHocGoc;
                if (lichHoc.NgayKetThuc < DateOnly.FromDateTime(DateTime.Now)) return false;
                if (lichHoc.NgayKetThuc == DateOnly.FromDateTime(DateTime.Now))
                {
                    if (lichHoc.GioKetThuc < TimeOnly.FromDateTime(DateTime.Now)) return false;
                }
            }
        }
        return true;
    }

    private async Task<bool> TonTaiDiemDanh(UpdateDiemDanhCommand command, List<UpdateDiemDanhDto> diemDanhs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        foreach (var item in diemDanhs)
        {
            if (item.Id != null)
            {
                var check = await _context.DiemDanhs
                    .AnyAsync(d => d.Id == item.Id
                    && d.Ngay == command.Ngay
                    && d.ThamGiaLopHoc.LichHoc.TenLop == command.TenLop
                    && d.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId);
                if (!check) return false;
            }
            else
            {
                var check = await _context.ThamGiaLopHocs
                    .AnyAsync(tg => tg.LichHoc.TenLop == command.TenLop
                    && tg.NgayBatDau!=DateOnly.MinValue
                    && tg.HocSinhCode == item.HocSinhCode
                    && tg.HocSinh.CoSoId == coSoId && tg.NgayBatDau <= command.Ngay
                    && tg.NgayKetThuc >= command.Ngay);
                if (!check) return false;
            }
        }
        return true;
    }

    private async Task<bool> DuocQuyenChinhSua(UpdateDiemDanhCommand command, List<UpdateDiemDanhDto> diemDanhs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichHoc = await _context.LichHocs
            .AnyAsync(lh => lh.TenLop == command.TenLop && lh.Phong.CoSoId == coSoId && lh.TrangThai == "Cố định");
        if (!lichHoc) return false;
        return true;
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanh;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
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
        RuleFor(x => x.UpdateDiemDanhs)
            .MustAsync(TonTaiDiemDanh)
            .WithMessage("Không tồn tại điểm danh này.")
            .MustAsync(CungChungMotLopVaNgay)
            .WithMessage("Các điểm danh phải cùng chung 1 lớp và ngày.")
            .MustAsync(DuocQuyenChinhSua)
            .WithMessage("Bạn không thể chỉnh sửa điểm danh này.")
            .MustAsync(ChuaHetHan)
            .WithMessage("Điểm danh này đã hết hạn chỉnh sửa.")
            .Must(DungFormatDiemDanh)
            .WithMessage("Điểm danh có format chưa đúng.");
    }

    private bool DungFormatDiemDanh(List<UpdateDiemDanhDto> list)
    {
        foreach (var dto in list)
        {
            if(dto.TrangThai!="Vắng"&&dto.TrangThai!="Có mặt") return false;
        }
        return true;
    }

    private async Task<bool> ChuaHetHan(List<UpdateDiemDanhDto> diemDanhs, CancellationToken token)
    {
        var idDiemDanh = diemDanhs.Select(d => d.Id).ToList();
        foreach(var item in idDiemDanh)
        {
            var DiemDanh = await _context.DiemDanhs
                .Include(d => d.ThamGiaLopHoc)
                .ThenInclude(tg => tg.LichHoc)
                .ThenInclude(lh => lh.LichHocGoc)
                
                .FirstOrDefaultAsync(d => d.Id == item);
            if (DiemDanh == null)
                return false;
            var lichHoc = DiemDanh.ThamGiaLopHoc.LichHoc;
            if(lichHoc == null) return false;
            if(lichHoc.TrangThai=="Dạy bù") lichHoc = lichHoc.LichHocGoc;
            if(lichHoc.NgayKetThuc<DateOnly.FromDateTime(DateTime.Now)) return false;
            if (lichHoc.NgayKetThuc == DateOnly.FromDateTime(DateTime.Now))
            {
                if(lichHoc.GioKetThuc < TimeOnly.FromDateTime(DateTime.Now)) return false;
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
        var coSoId = _identityService.GetCampusId(token);
        var idDiemDanh = diemDanhs.Select(d => d.Id).ToList();
        foreach(var id in idDiemDanh)
        {
            var diemDanh = await _context.DiemDanhs
            .FirstOrDefaultAsync(dd => dd.Id == id
            && (dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId));
            if (diemDanh == null) return false;
        }
        return true;
    }
}

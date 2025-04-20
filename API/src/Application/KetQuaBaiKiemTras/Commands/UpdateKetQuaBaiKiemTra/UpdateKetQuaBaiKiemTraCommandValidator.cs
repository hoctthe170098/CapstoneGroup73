using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace StudyFlow.Application.KetQuaBaiKiemTras.Commands.UpdateKetQuaBaiKiemTra;

public class UpdateKetQuaBaiKiemTraCommandValidator : AbstractValidator<UpdateKetQuaBaiKiemTraCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateKetQuaBaiKiemTraCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.UpdateKetQuas)
            .MustAsync(TonTaiKetQua)
            .WithMessage("Không tồn tại kết quả này")
            .MustAsync(CungChungMotBaiKiemTra)
            .WithMessage("Các kết quả phải cùng chung 1 bài kiểm tra")
            .MustAsync(DuocQuyenChinhSua)
            .WithMessage("Bạn không thể chỉnh sửa điểm danh này")
            .Must(DungFormatKetQua)
            .WithMessage("Điểm danh có format chưa đúng");
    }

    private async Task<bool> TonTaiKetQua(List<KetQuaBaiKiemTraDto> list, CancellationToken token)
    {
        foreach (var item in list)
        {
            var check = await _context.KetQuaBaiKiemTras.AnyAsync(kq => kq.Id == item.Id);
            if (!check) return false;
        }
        return true;
    }

    private bool DungFormatKetQua(List<KetQuaBaiKiemTraDto> list)
    {
        foreach (var dto in list)
        {
            if(dto.Diem!=null&&dto.Diem>10||dto.Diem<0) return false;
            if(dto.NhanXet!=null&&dto.NhanXet!.Length>200) return false;
        }
        return true;
    }
    private async Task<bool> CungChungMotBaiKiemTra(List<KetQuaBaiKiemTraDto> ketQuas, CancellationToken token)
    {
        var ketQuaBaiKiemTras = await _context.KetQuaBaiKiemTras
            .Where(d=> ketQuas.Select(dd=>dd.Id).ToList().Contains(d.Id))
            .Select(d=>d.BaiKiemTraId)
            .Distinct()
            .ToListAsync();
        return ketQuaBaiKiemTras.Count() == 1;
    }
    private async Task<bool> DuocQuyenChinhSua(List<KetQuaBaiKiemTraDto> ketQuas, CancellationToken cToken)
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
        var idKetQua = ketQuas.Select(d => d.Id).ToList();
        foreach(var id in idKetQua)
        {
            var ketQua = await _context.KetQuaBaiKiemTras
                .Include(kq=>kq.BaiKiemTra)
                .ThenInclude(kt=>kt.LichHoc)
            .FirstOrDefaultAsync(dd => dd.Id == id);
            if(ketQua == null) return false;
            if (ketQua.BaiKiemTra.LichHoc.GiaoVienCode != giaoVien.Code) return false;
        }
        return true;
    }
}

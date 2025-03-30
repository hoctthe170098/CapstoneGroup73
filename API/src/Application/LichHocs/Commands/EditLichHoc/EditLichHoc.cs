using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public record EditLichHocCommand : IRequest<Output>
{
    public required EditLichHocDto LopHocDto { get; init; }
}

public class EditLichHocCommandHandler : IRequestHandler<EditLichHocCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditLichHocCommandHandler(IApplicationDbContext context, IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(EditLichHocCommand request, CancellationToken cancellationToken)
    {
        var daBatDau = await _context.LichHocs
            .AnyAsync(lh => lh.NgayBatDau >= DateOnly.FromDateTime(DateTime.Now));
        if(daBatDau)
        {
            foreach(var lh in request.LopHocDto.LichHocs)
            {
                if (lh.Id == null) break;
                var lichHocData = await _context.LichHocs.FirstAsync(lh=>lh.Id==lh.Id);
                lichHocData.PhongId = lh.PhongId;
                await _context.SaveChangesAsync(cancellationToken);
                var thamGiaCanXoaOrUpdate = _context.ThamGiaLopHocs
                   .Where(tg => tg.LichHocId == lh.Id && !request.LopHocDto.HocSinhCodes
                                                                     .Contains(tg.HocSinhCode))
                   .ToList();
                foreach(var thamGia in thamGiaCanXoaOrUpdate)
                {
                    if(await _context.DiemDanhs.AnyAsync(dh => dh.ThamGiaLopHocId == thamGia.Id))
                    {
                        thamGia.NgayKetThuc = DateOnly.FromDateTime(DateTime.Now);
                    }
                    else
                    {
                        _context.ThamGiaLopHocs.Remove(thamGia);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }
                var codeHocSinhCanThem = request.LopHocDto.HocSinhCodes
                    .Where(hs => !_context.ThamGiaLopHocs
                                         .Where(tg => tg.LichHocId == lh.Id)
                                         .Select(tg => tg.HocSinhCode)
                                         .Contains(hs))
                    .ToList();
                foreach (var code in codeHocSinhCanThem)
                {
                    var thamGiaLopHoc = new ThamGiaLopHoc
                    {
                        Id = Guid.NewGuid(),
                        HocSinhCode = code,
                        LichHocId = (Guid)lh.Id,
                        NgayBatDau = DateOnly.FromDateTime(DateTime.Now),
                        NgayKetThuc = request.LopHocDto.NgayKetThuc,
                        TrangThai = "Cố định"
                    };
                    _context.ThamGiaLopHocs.Add(thamGiaLopHoc);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            var lichHocData = await _context.LichHocs
                .Where(lh => lh.TenLop == request.LopHocDto.TenLop)
                .ToListAsync();
            var lichHocCanUpDate = request.LopHocDto.LichHocs
                .Where(l => l.Id != null)
                .ToList();
            var lichHocCanXoa = lichHocData
                .Where(lh=>!lichHocCanUpDate.Select(l => l.Id).Contains(lh.Id))
                .ToList();
            var lichHocCanThem = request.LopHocDto.LichHocs.Where(lh=>lh.Id==null).ToList();
            foreach(var lh in lichHocCanUpDate)
            {
                var lichHoc = _context.LichHocs.Where(l=>l.Id==lh.Id).First();
                lichHoc.Thu = lh.Thu;
                lichHoc.GioBatDau = TimeOnly.Parse(lh.GioBatDau);
                lichHoc.GioKetThuc = TimeOnly.Parse(lh.GioKetThuc);
                lichHoc.PhongId = lh.PhongId;
                await _context.SaveChangesAsync(cancellationToken);
            }
            _context.LichHocs.RemoveRange(lichHocCanXoa);
            await _context.SaveChangesAsync(cancellationToken);
            foreach(var lh in lichHocCanThem)
            {
                LichHoc lichHoc = new LichHoc {
                    Id = Guid.NewGuid(),
                    ChuongTrinhId = request.LopHocDto.ChuongTrinhId,
                    GiaoVienCode = request.LopHocDto.GiaoVienCode,
                    GioBatDau = TimeOnly.Parse(lh.GioBatDau),
                    GioKetThuc = TimeOnly.Parse(lh.GioKetThuc),
                    HocPhi = request.LopHocDto.HocPhi,
                    NgayBatDau = request.LopHocDto.NgayBatDau,
                    NgayKetThuc = request.LopHocDto.NgayKetThuc,
                    PhongId = lh.PhongId,
                    TenLop = request.LopHocDto.TenLop,
                    Thu = lh.Thu,
                    TrangThai = "Cố định"
                };
                _context.LichHocs.Add(lichHoc);
                await _context.SaveChangesAsync(cancellationToken);
            }
            var lichHocSauUpdate = await _context.LichHocs
                .Where(lh=>lh.TenLop==request.LopHocDto.TenLop)
                .ToListAsync();
            foreach(var lh in lichHocSauUpdate)
            {
                var codeHocSinhCanXoa = _context.ThamGiaLopHocs
                    .Where(tg=>tg.LichHocId==lh.Id&&!request.LopHocDto.HocSinhCodes
                                                                      .Contains(tg.HocSinhCode))
                    .ToList();
                _context.ThamGiaLopHocs.RemoveRange(codeHocSinhCanXoa);
                await _context.SaveChangesAsync(cancellationToken);
                var codeHocSinhCanThem = request.LopHocDto.HocSinhCodes
                    .Where(hs => !_context.ThamGiaLopHocs
                                         .Where(tg => tg.LichHocId == lh.Id)
                                         .Select(tg => tg.HocSinhCode)
                                         .Contains(hs))
                    .ToList();
                foreach(var code in codeHocSinhCanThem)
                {
                    var thamGiaLopHoc = new ThamGiaLopHoc { 
                        Id = Guid.NewGuid(),
                        HocSinhCode = code,
                        LichHocId = lh.Id,
                        NgayBatDau = DateOnly.FromDateTime(DateTime.Now),
                        NgayKetThuc = lh.NgayKetThuc,
                        TrangThai = "Cố định"         
                    };
                    _context.ThamGiaLopHocs.Add(thamGiaLopHoc);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
        return new Output
        {
            code = 200,
            isError = false,
            message = "Chỉnh sửa lớp thành công"
        };
    }
}

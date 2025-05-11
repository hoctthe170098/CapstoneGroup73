using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using StudyFlow.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var daBatDau = await _context.LichHocs
            .AnyAsync(lh => lh.TenLop == request.LopHocDto.TenLop 
            && lh.Phong.CoSoId == coSoId
            && lh.TrangThai == "Cố định" 
            && lh.NgayBatDau <= ngayHienTai);
        if(daBatDau)
        {
            var lichHocCu = _context.LichHocs
            .Where(lh => lh.TenLop == request.LopHocDto.TenLop
            && lh.Phong.CoSoId == coSoId
            && lh.TrangThai=="Cố định")
            .ToList();
            foreach (var lh in lichHocCu)
            {
                var lichHocData = request.LopHocDto.LichHocs.First(l=>l.Id==lh.Id);
                lh.PhongId = lichHocData.PhongId;
                var thamGiaCanXoaOrUpdate = _context.ThamGiaLopHocs
                   .Where(tg => (tg.LichHocId == lh.Id
                   ||(tg.LichHoc.LichHocGocId==lh.Id&&tg.LichHoc.TrangThai=="Dạy bù")) 
                   && !request.LopHocDto.HocSinhCodes.Contains(tg.HocSinhCode))
                   .Include(tg=>tg.LichHoc)
                   .ToList();
                foreach(var thamGia in thamGiaCanXoaOrUpdate)
                {
                    if (thamGia.LichHoc.NgayBatDau <= ngayHienTai 
                        && thamGia.LichHoc.NgayKetThuc >= ngayHienTai 
                        && thamGia.LichHoc.TrangThai=="Cố định")
                    {
                        thamGia.NgayKetThuc = DateOnly.FromDateTime(DateTime.Now);
                    }else if(thamGia.LichHoc.TrangThai=="Dạy bù")
                    {
                        if (thamGia.NgayKetThuc > ngayHienTai)
                        {
                            _context.ThamGiaLopHocs.Remove(thamGia);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                    else
                    {
                        _context.ThamGiaLopHocs.Remove(thamGia);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
                var codeHocSinhCanThem = request.LopHocDto.HocSinhCodes
                    .Where(hs => !_context.ThamGiaLopHocs
                                         .Where(tg => tg.LichHocId == lh.Id&&tg.LichHoc.TrangThai=="Cố định"&&tg.NgayKetThuc>ngayHienTai)
                                         .Select(tg => tg.HocSinhCode)
                                         .Contains(hs))
                    .ToList();
                foreach (var code in codeHocSinhCanThem)
                {
                    var thamGiaLopHocCu = _context.ThamGiaLopHocs
                        .FirstOrDefault(tg=>tg.HocSinhCode==code&&tg.LichHocId==lh.Id);
                    if(thamGiaLopHocCu != null)
                    {
                        thamGiaLopHocCu.NgayKetThuc = lh.NgayKetThuc;
                    }
                    else
                    {
                        var thamGiaLopHoc = new ThamGiaLopHoc
                        {
                            Id = Guid.NewGuid(),
                            HocSinhCode = code,
                            LichHocId = (Guid)lh.Id,
                            NgayBatDau = DateOnly.FromDateTime(DateTime.Now),
                            NgayKetThuc = request.LopHocDto.NgayKetThuc,
                            TrangThai = lh.TrangThai
                        };
                        _context.ThamGiaLopHocs.Add(thamGiaLopHoc);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    var lichHocBuTuongLai = await _context.LichHocs
                        .Where(l=>l.LichHocGocId==lh.Id
                        &&l.TrangThai=="Dạy bù"
                        &&l.NgayKetThuc>ngayHienTai)
                        .ToListAsync();
                    foreach(var hocBu in lichHocBuTuongLai)
                    {
                        var thamGiaLopHocBu = new ThamGiaLopHoc
                        {
                            Id = Guid.NewGuid(),
                            HocSinhCode = code,
                            LichHocId = (Guid)hocBu.Id,
                            NgayBatDau = hocBu.NgayBatDau,
                            NgayKetThuc = hocBu.NgayKetThuc,
                            TrangThai = hocBu.TrangThai
                        };
                        _context.ThamGiaLopHocs.Add(thamGiaLopHocBu);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var lichHocData = await _context.LichHocs
                .Where(lh => lh.TenLop == request.LopHocDto.TenLop 
                && lh.Phong.CoSoId == coSoId)
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
                var lichHoc = _context.LichHocs
                    .Where(l=>l.Id==lh.Id && l.Phong.CoSoId == coSoId).First();
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
                .Where(lh=>lh.TenLop == request.LopHocDto.TenLop 
                && lh.Phong.CoSoId == coSoId)
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

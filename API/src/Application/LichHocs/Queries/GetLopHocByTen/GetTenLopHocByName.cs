using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.HocSinhs.Queries.GetHocSinhsByNameOrCode;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Application.LichHocs.Queries.GetTenLopHocByName;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocByTen;
public record GetLopHocByTenQuery : IRequest<Output>
{
    public required string TenLop { get; init; } = "";
}
public class GetLopHocByTenQueryHandler : IRequestHandler<GetLopHocByTenQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetLopHocByTenQueryHandler(IApplicationDbContext context
        ,IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService
        , IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetLopHocByTenQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichHoc = await _context.LichHocs
            .Where(lh=>lh.TenLop==request.TenLop&&lh.TrangThai=="Cố định"&&lh.Phong.CoSoId==coSoId)
            .Include(lh=>lh.GiaoVien)
            .Include(lh=>lh.ChuongTrinh)
            .ToListAsync();
        var daHoc = _context.DiemDanhs.Any(dh => dh.ThamGiaLopHoc.LichHoc.TenLop == request.TenLop);
        if (lichHoc.Count == 0) throw new NotFoundIDException();
        LopHocDto lopHoc = new LopHocDto
        {
            TenLop = request.TenLop,
            GiaoVienCode = lichHoc[0].GiaoVienCode,
            TenGiaoVien = lichHoc[0].GiaoVien.Ten,
            NgayBatDau = lichHoc[0].NgayBatDau,
            NgayKetThuc = lichHoc[0].NgayKetThuc,
            ChuongTrinhId = lichHoc[0].ChuongTrinhId,
            TenChuongTrinh = lichHoc[0].ChuongTrinh.TieuDe,
            HocPhi = lichHoc[0].HocPhi,
            LichHocs = new List<LichHocDto>(),
            HocSinhs = new List<HocSinhDto>(),
            DaHoc = daHoc,
        };
        foreach(var lh in lichHoc)
        {
            LichHocDto lichhoc = new LichHocDto
            {
                Id = lh.Id,
                GioBatDau = lh.GioBatDau,
                GioKetThuc = lh.GioKetThuc,
                PhongId = lh.PhongId,
                Thu = lh.Thu
            };
            lopHoc.LichHocs.Add(lichhoc);
        }
        var listHocSinh = _context.HocSinhs
            .Where(hs=>hs.ThamGiaLopHocs
            .Any(tg=>tg.LichHoc.TenLop==request.TenLop&&tg.NgayKetThuc>DateOnly.FromDateTime(DateTime.Now)&&tg.LichHoc.TrangThai=="Cố định"))
            .Distinct();
        if (listHocSinh.Any())
        {
            var hocSinh = _mapper.Map<List<HocSinhDto>>(listHocSinh);
            lopHoc.HocSinhs = hocSinh.ToList();
        }
        return new Output
        {
            isError = false,
            code = 200,
            data = lopHoc,
            message = "Lấy danh sách tên lớp thành công"
        };
    }
}

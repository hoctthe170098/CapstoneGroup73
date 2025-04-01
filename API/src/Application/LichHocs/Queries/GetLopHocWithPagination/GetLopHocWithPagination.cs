using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
public record GetLopHocWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize {  get; init; } = 3;
    public required string TenLop { get; init; } = "";
    public required List<int> Thus { get; init; } = new List<int>();
    public required string GiaoVienCode { get; init; } = "all";
    public required int PhongId { get; init; } = 0;
    public required int ChuongTrinhId { get; init; } = 0;
    public required string TrangThai { get; init; } = "all";
    public required string ThoiGianBatDau { get; init; } = "";
    public required string ThoiGianKetThuc { get; init; } = "";
    public required DateOnly NgayBatDau { get; init; } = DateOnly.MinValue;
    public required DateOnly NgayKetThuc { get; init; } = DateOnly.MinValue;

}
public class GetLopHocWithPaginationQueryHandler : IRequestHandler<GetLopHocWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetLopHocWithPaginationQueryHandler(IApplicationDbContext context
        ,IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetLopHocWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var query =  _context.LichHocs
            .Where(q=>q.Phong.CoSoId == coSoId)
            .AsQueryable();
        if (request.TenLop != "")
        {
            query = query.Where(lh => lh.TenLop.Contains(request.TenLop));
        }
        if(request.Thus.Count() > 0)
        {
            query = query.Where(lh=>request.Thus.Contains(lh.Thu));
        }
        if (request.ChuongTrinhId != 0)
        {
            query = query.Where(lh => lh.ChuongTrinhId == request.ChuongTrinhId);
        }
        if (request.TrangThai.ToLower().Trim() != "all")
        {
            query = query.Where(lh=>request.TrangThai.ToLower().Trim()==lh.TrangThai);
        }
        if (request.GiaoVienCode.ToLower().Trim() != "all")
        {
            query = query.Where(lh=>lh.GiaoVienCode==request.GiaoVienCode);
        }
        if(request.PhongId!=0)
        {
            query = query.Where(lh=>lh.PhongId==request.PhongId);
        }
        if (request.ThoiGianBatDau.Trim() != "")
        {
            var tgBatDau = TimeOnly.Parse(request.ThoiGianBatDau);
            query = query.Where(lh => lh.GioBatDau >= tgBatDau);
        }
        if (request.ThoiGianKetThuc.Trim() != "")
        {
            var tgKetThuc = TimeOnly.Parse(request.ThoiGianKetThuc);
            query = query.Where(lh => lh.GioKetThuc <= tgKetThuc);
        }
        if(request.ThoiGianBatDau.Trim()!=""&& request.ThoiGianKetThuc.Trim() != "")
        {
            var tgBatDau = TimeOnly.Parse(request.ThoiGianBatDau);
            var tgKetThuc = TimeOnly.Parse(request.ThoiGianKetThuc);
            query = query.Where(lh => lh.GioBatDau >= tgBatDau && lh.GioKetThuc <= tgKetThuc);
        }
        if (request.NgayBatDau != DateOnly.MinValue)
        {
            query = query.Where(lh => lh.NgayBatDau >= request.NgayBatDau);
        }
        if (request.NgayKetThuc != DateOnly.MinValue)
        {
            query = query.Where(lh => lh.NgayKetThuc <= request.NgayKetThuc);
        }
        if (request.NgayBatDau != DateOnly.MinValue && request.NgayKetThuc != DateOnly.MinValue)
        {
            query = query.Where(lh => lh.NgayBatDau >= request.NgayBatDau 
                && lh.NgayKetThuc <= request.NgayKetThuc);
        }
        var listClass = query.Select(lh => lh.TenLop).Distinct().ToList();
        var listClassPagging = listClass
            .Skip((request.PageNumber-1)*request.PageSize)
            .Take(request.PageSize)
            .ToList();
        LopHocWithPaginationDTO lopHocWithPaginationDTO = new LopHocWithPaginationDTO
        {
            PageNumber = request.PageNumber,
            TotalCount = listClass.Count,
            TotalPages = (listClass.Count % request.PageSize == 0)
            ? listClass.Count / request.PageSize : listClass.Count / request.PageSize + 1,
            LopHocs = new List<LopHocDto>()
        };
        foreach (var cla in listClassPagging)
        {
            var lichHocs = await _context.LichHocs
                .Include(lh=>lh.GiaoVien)
                .Include(lh=>lh.Phong)
                .Include(lh=>lh.ChuongTrinh)
                .Where(lh=>lh.TenLop== cla).ToListAsync();
            LopHocDto lopHoc = new LopHocDto
            {
                HocPhi = lichHocs[0].HocPhi,
                TenChuongTrinh = lichHocs[0].ChuongTrinh.TieuDe,
                TenGiaoVien = lichHocs[0].GiaoVien.Ten,
                TenLop = cla,
                LoaiLichHocs = new List<LoaiLichHocDto>()
            };
            lopHoc.LoaiLichHocs.Add(GetListLichHoc(lichHocs, "Cố định"));
            lopHoc.LoaiLichHocs.Add(GetListLichHoc(lichHocs, "Dạy bù"));
            lopHoc.LoaiLichHocs.Add(GetListLichHoc(lichHocs, "Dạy thay"));
            lopHocWithPaginationDTO.LopHocs.Add(lopHoc);
        }
        return new Output
        {
            isError = false,
            code = 200,
            data = lopHocWithPaginationDTO,
            message = "Lấy danh sách lớp học thành công"
        };
    }
    private LoaiLichHocDto GetListLichHoc(List<LichHoc>lichHocs,string trangThai)
    {
        var LichHocs = lichHocs.Where(lh => lh.TrangThai == trangThai).ToList();
            LoaiLichHocDto LoaiLichHoc = new LoaiLichHocDto
            {
                TrangThai = trangThai,
                LichHocs = new List<LichHocDto>()
            };
            foreach (var lichHoc in LichHocs)
            {
                LichHocDto lich = new LichHocDto
                {
                    Id = lichHoc.Id,
                    gioBatDau = lichHoc.GioBatDau,
                    gioKetThuc = lichHoc.GioKetThuc,
                    ngayBatDau = lichHoc.NgayBatDau,
                    ngayKetThuc = lichHoc.NgayKetThuc,
                    TenPhong = lichHoc.Phong.Ten,
                    Thu = lichHoc.Thu
                };
            LoaiLichHoc.LichHocs.Add(lich);
            }
        return LoaiLichHoc;
    }
}

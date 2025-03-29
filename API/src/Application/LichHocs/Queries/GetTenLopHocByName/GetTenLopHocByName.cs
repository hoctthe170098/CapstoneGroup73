using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Application.LichHocs.Queries.GetTenLopHocByName;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocByName;
public record GetLopHocByNameQuery : IRequest<Output>
{
    public required string TenLop { get; init; } = "";
}
public class GetLopHocByNameQueryHandler : IRequestHandler<GetLopHocByNameQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetLopHocByNameQueryHandler(IApplicationDbContext context
        ,IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetLopHocByNameQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var listTen = await _context.LichHocs.Where(lh=>lh.Phong.CoSoId == coSoId)
            .Select(lh=>lh.TenLop).Distinct()
            .ToListAsync();
        if (request.TenLop.Trim() != "")
        {
            listTen = listTen
                .Where(t=>t.ToLower().Trim().Contains(request.TenLop.ToLower().Trim()))
                .ToList();
        }
        List<LopHocByNameDto> lopHocs = new List<LopHocByNameDto>();
        foreach(var tenLop in listTen)
        {
            LopHocByNameDto lop = new LopHocByNameDto
            {
                TenLop = tenLop,
                LichHocs = new List<LichHocByNameDto>()
            };
            var lichHoc = _context.LichHocs.Where(lh=>lh.TenLop == tenLop&&lh.NgayKetThuc> DateOnly.FromDateTime(DateTime.Now))
                .Include(lh=>lh.Phong)
                .OrderBy(lh=>lh.TrangThai)
                .ToList();
            foreach(var lich in lichHoc)
            {
                lop.LichHocs.Add(new LichHocByNameDto
                {
                    TenPhong = lich.Phong.Ten,
                    GioBatDau = lich.GioBatDau,
                    GioKetThuc = lich.GioKetThuc,
                    NgayBatDau= lich.NgayBatDau,
                    NgayKetThuc = lich.NgayKetThuc,
                    Thu = lich.Thu,
                    TrangThai = lich.TrangThai
                });
            }
            lopHocs.Add(lop);
        }
        return new Output
        {
            isError = false,
            code = 200,
            data = lopHocs,
            message = "Lấy danh sách tên lớp thành công"
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ThamGiaLopHocs.Queries.GetHocSinhAssignedClass;
public record GetHocSinhAssignedClassWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 9;
    public string? SearchClass { get; init; }
}

public class GetHocSinhAssignedClassWithPaginationHandler
    : IRequestHandler<GetHocSinhAssignedClassWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetHocSinhAssignedClassWithPaginationHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = contextAccessor;
    }

    public async Task<Output> Handle(GetHocSinhAssignedClassWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var userId = _identityService.GetUserId(token);
        var coSoId = _identityService.GetCampusId(token);
        var hocSinh = await _context.HocSinhs
            .FirstOrDefaultAsync(hs => hs.UserId == userId.ToString(), cancellationToken);

        if (hocSinh == null)
            throw new Exception("Không tìm thấy giáo viên tương ứng với tài khoản.");

        var query = _context.ThamGiaLopHocs
            .Where(t => t.HocSinhCode == hocSinh.Code && t.LichHoc.TrangThai == "Cố định"&&t.LichHoc.Phong.CoSoId==coSoId)
            .Select(t => t.LichHoc)
            .GroupBy(l => l.TenLop)
            .Select(g => new LichHoc
            {
                Id = g.First().Id,
                Thu = g.First().Thu,
                PhongId = g.First().PhongId,
                TenLop = g.Key, // Lấy key chính là TenLop
                NgayBatDau = g.First().NgayBatDau,
                NgayKetThuc = g.First().NgayKetThuc,
                HocPhi = g.First().HocPhi,
                TrangThai = g.First().TrangThai,
                GiaoVienCode = g.First().GiaoVienCode,
                ChuongTrinhId = g.First().ChuongTrinhId,
                GioBatDau = g.First().GioBatDau,
                GioKetThuc = g.First().GioKetThuc
            }).AsQueryable();


        if (!string.IsNullOrWhiteSpace(request.SearchClass))
        {
            query = query.Where(x => x.TenLop.Contains(request.SearchClass));   
        }

        var list = await query
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return new Output
        {
            isError = false,
            data = list,
            message = "Lấy danh sách lớp thành công",
            code = 200
        };
    }
}

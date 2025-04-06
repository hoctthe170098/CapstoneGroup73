using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Queries.GetGiaoVienAssignedClass;
public record GetGiaoVienAssignedClassWithPaginationCommand : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 9;
    public string? SearchClass { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
}

public class GetGiaoVienAssignedClassWithPaginationCommandHandler
    : IRequestHandler<GetGiaoVienAssignedClassWithPaginationCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetGiaoVienAssignedClassWithPaginationCommandHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetGiaoVienAssignedClassWithPaginationCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ request header
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var userId = _identityService.GetUserId(token);

        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == userId.ToString(), cancellationToken);

        if (giaoVien == null)
            throw new Exception("Không tìm thấy giáo viên tương ứng với tài khoản.");

        var query = _context.LichHocs
            .Where(lh => lh.GiaoVienCode == giaoVien.Code && lh.TrangThai != "Dạy thay")
            .GroupBy(lh => lh.TenLop)
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
            query = query.Where(lh => lh.TenLop.Contains(request.SearchClass));
        }

        var danhSachTenLop = await query
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return new Output
        {
            isError = false,
            data = danhSachTenLop,
            code = 200
        };
    }
}

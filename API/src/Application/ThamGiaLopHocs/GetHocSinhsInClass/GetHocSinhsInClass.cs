using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ThamGiaLopHocs.GetHocSinhsInClass;
public record GetHocSinhsInClassQueries : IRequest<Output>
{
    public required string TenLop { get; init; }
}

public class GetHocSinhsInClassQueriesHandler : IRequestHandler<GetHocSinhsInClassQueries, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetHocSinhsInClassQueriesHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetHocSinhsInClassQueries request, CancellationToken cancellationToken)
    {

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var userId = _identityService.GetUserId(token);

        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == userId.ToString(), cancellationToken);

        if (giaoVien == null)
            throw new Exception("Không tìm thấy giáo viên tương ứng với tài khoản.");

        var list = await _context.ThamGiaLopHocs
            .Include(t => t.LichHoc)
            .Where(t => t.LichHoc.TenLop == request.TenLop && t.LichHoc.GiaoVienCode == giaoVien.Code)
            .Select(t => t.HocSinh)
            .Distinct()
            .OrderByDescending(hs => hs.Ten)
            .ToListAsync(cancellationToken);

        if (list == null || !list.Any())
            throw new NotFoundIDException();

        return new Output
        {
            isError = false,
            data = list,
            message = "Lấy danh sách học sinh thành công"
        };

    }
}

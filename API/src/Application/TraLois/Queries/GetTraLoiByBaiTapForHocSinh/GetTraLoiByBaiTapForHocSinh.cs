using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.TraLois.Queries.GetTraLoiByBaiTapForHocSinh;
public class GetTraLoiByBaiTapForHocSinh : IRequest<Output>
{
    public required Guid BaiTapId { get; init; }
}
public class GetTraLoiByBaiTapForHocSinhHandler : IRequestHandler<GetTraLoiByBaiTapForHocSinh, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetTraLoiByBaiTapForHocSinhHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetTraLoiByBaiTapForHocSinh request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy học sinh.");

        var traLois = await _context.TraLois
            .AsNoTracking()
            .Include(t => t.HocSinh)
            .Where(t => t.BaiTapId == request.BaiTapId && t.HocSinhCode == hocSinh.Code)
            .OrderByDescending(t => t.ThoiGian)
            .Select(t => new
            {
                t.Id,
                t.NoiDung,
                t.UrlFile,
                t.ThoiGian,
                t.Diem,
                t.NhanXet,
                HocSinhTen = t.HocSinh.Ten
            })
            .ToListAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            message = "Lấy danh sách trả lời thành công",
            data = traLois
        };
    }
}

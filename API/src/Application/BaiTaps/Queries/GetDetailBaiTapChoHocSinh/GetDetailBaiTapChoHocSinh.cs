using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Queries.GetDetailBaiTapChoHocSinh;

public record GetDetailBaiTapChoHocSinhQuery : IRequest<Output>
{
    public Guid BaiTapId { get; init; }
}

public class GetDetailBaiTapChoHocSinhQueryHandler : IRequestHandler<GetDetailBaiTapChoHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetDetailBaiTapChoHocSinhQueryHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetDetailBaiTapChoHocSinhQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var baiTap = await _context.BaiTaps
            .Include(bt => bt.LichHoc)
                .ThenInclude(lh => lh.ThamGiaLopHocs)
            .FirstOrDefaultAsync(bt => bt.Id == request.BaiTapId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy bài tập.");

        var isHocSinh = baiTap.LichHoc.ThamGiaLopHocs
            .Any(tg => tg.HocSinhCode == hocSinh.Code);

        if (!isHocSinh)
            throw new UnauthorizedAccessException("Bạn không có quyền truy cập bài tập này.");

        int secondsLeft = 0;

        if (baiTap.ThoiGianKetThuc.HasValue)
        {
            var now = DateTime.Now;
            var timeLeft = baiTap.ThoiGianKetThuc.Value - now;
            secondsLeft = (int)Math.Max(timeLeft.TotalSeconds, 0);
        }

        var result = new
        {
            baiTap.Id,
            baiTap.TieuDe,
            baiTap.NoiDung,
            NgayTao = baiTap.NgayTao.ToString("yyyy-MM-dd"),
            ThoiGianKetThuc = baiTap.ThoiGianKetThuc?.ToString("yyyy-MM-ddTHH:mm:ss"),
            SecondsUntilDeadline = secondsLeft,
            UrlFile = baiTap.UrlFile,
            TrangThai = baiTap.TrangThai
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy chi tiết bài tập thành công."
        };
    }
}



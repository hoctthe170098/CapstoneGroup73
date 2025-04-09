using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Queries.GetDetailBaiTapChoGiaoVien;
public record GetBaiTapDetailQuery() : IRequest<Output>
    {
    public Guid BaiTapId { get; init; }
}
public class GetBaiTapDetailQueryHandler : IRequestHandler<GetBaiTapDetailQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaiTapDetailQueryHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetBaiTapDetailQuery request, CancellationToken cancellationToken)
    {
        // Lấy token từ header và xác thực người dùng
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        // Lấy giáo viên từ userId
        var giaoVien = await _context.GiaoViens
            .AsNoTracking()
            .FirstOrDefaultAsync(gv => gv.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy giáo viên tương ứng với tài khoản.");

        // Tìm bài tập 
        var baiTap = await _context.BaiTaps
            .AsNoTracking()
            .Include(bt => bt.LichHoc)
            .FirstOrDefaultAsync(bt => bt.Id == request.BaiTapId, cancellationToken);

        if (baiTap == null)
            throw new NotFoundDataException("Không tìm thấy bài tập.");

        //  Kiểm tra quyền sở hữu bài tập 
        if (baiTap.LichHoc.GiaoVienCode != giaoVien.Code)
            throw new UnauthorizedAccessException("Bạn không có quyền xem bài tập này.");

        //  Tính thời gian còn lại
        int secondsLeft = 0;
        if (baiTap.ThoiGianKetThuc.HasValue)
        {
            var now = DateTime.Now;
            var timeLeft = baiTap.ThoiGianKetThuc.Value - now;
            secondsLeft = (int)Math.Max(timeLeft.TotalSeconds, 0);
        }

        //  Chuẩn bị dữ liệu trả về
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

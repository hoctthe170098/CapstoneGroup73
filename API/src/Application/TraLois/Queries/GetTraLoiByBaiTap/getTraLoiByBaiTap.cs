using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.TraLois.Queries.GetBaiTapByTraLoi;

public class GetTraLoiByBaiTapQuery : IRequest<Output>
{
    public required Guid BaiTapId { get; init; }
}

public class GetTraLoiByBaiTapQueryHandler : IRequestHandler<GetTraLoiByBaiTapQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetTraLoiByBaiTapQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetTraLoiByBaiTapQuery request, CancellationToken cancellationToken)
    {
        if (request.BaiTapId == Guid.Empty)
        {
            return new Output
            {
                isError = true,
                code = 400,
                message = "ID bài tập không hợp lệ"
            };
        }

        // Lấy token và userId
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        // Kiểm tra xem là giáo viên hay học sinh
        var isTeacher = await _context.GiaoViens.AnyAsync(gv => gv.UserId == userId, cancellationToken);
        var isStudent = !isTeacher;

        // Query cơ bản
        var query = _context.TraLois
        .AsNoTracking()
        .Include(t => t.HocSinh)
        .Where(t => t.BaiTapId == request.BaiTapId);

        if (isStudent)
        {
            var hocSinh = await _context.HocSinhs
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken);

            if (hocSinh == null)
            {
                return new Output
                {
                    isError = true,
                    code = 404,
                    message = "Không tìm thấy thông tin học sinh."
                };
            }

            query = query.Where(t => t.HocSinhCode == hocSinh.Code);
        }

        // Thêm sắp xếp sau khi đã filter xong
        var data = await query
            .OrderByDescending(t => t.ThoiGian)
            .Select(t => new TraLoiDto
            {
                Id = t.Id,
                NoiDung = t.NoiDung,
                UrlFile = t.UrlFile,
                ThoiGian = t.ThoiGian,
                HocSinhTen = t.HocSinh.Ten
            })
            .ToListAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = data,
            message = "Lấy danh sách trả lời thành công"
        };
    }
}

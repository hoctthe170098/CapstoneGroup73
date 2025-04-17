using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.TraLois.Queries.GetBaiTapByTraLoi;

public class GetTraLoiByBaiTapQuery : IRequest<Output>
{
    public required Guid BaiTapId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
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

        if (request.PageNumber < 1 || request.PageSize < 1)
            throw new Exception("Số trang hoặc kích thước không hợp lệ!");

        // Lấy token và userId
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        // Chỉ cho giáo viên truy cập
        var isTeacher = await _context.GiaoViens.AnyAsync(gv => gv.UserId == userId, cancellationToken);
        if (!isTeacher)
        {
            return new Output
            {
                isError = true,
                code = 403,
                message = "Bạn không có quyền truy cập thông tin này."
            };
        }

        // Query trả lời
        var query = _context.TraLois
            .AsNoTracking()
            .Include(t => t.HocSinh)
            .Where(t => t.BaiTapId == request.BaiTapId);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var data = await query
            .OrderByDescending(t => t.ThoiGian)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TraLoiDto
            {
                Id = t.Id,
                NoiDung = t.NoiDung,
                UrlFile = t.UrlFile,
                HocSinhCode = t.HocSinhCode,
                Diem = t.Diem,
                NhanXet = t.NhanXet,
                ThoiGian = t.ThoiGian,
                HocSinhTen = t.HocSinh.Ten
            })
            .ToListAsync(cancellationToken);

        var result = new
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = data
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy danh sách trả lời thành công"
        };
    }
}

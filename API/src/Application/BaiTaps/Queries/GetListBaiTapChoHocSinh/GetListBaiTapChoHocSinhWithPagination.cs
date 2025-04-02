using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Queries.TeacherAssignmentList;

public record TeacherAssignmentListWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class TeacherAssignmentListWithPaginationQueryHandler : IRequestHandler<TeacherAssignmentListWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TeacherAssignmentListWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(TeacherAssignmentListWithPaginationQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
            throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");

        // Lấy studentCode từ token
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            throw new UnauthorizedAccessException("Người dùng chưa xác thực.");

        var studentCode = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(studentCode))
            throw new UnauthorizedAccessException("Không tìm thấy mã học sinh trong token.");

        // Truy vấn bài tập thuộc lớp học mà học sinh tham gia
        var query = _context.BaiTaps
            .AsNoTracking()
            .Where(bt => bt.LichHoc.ThamGiaLopHocs.Any(tg => tg.HocSinhCode == studentCode))
            .OrderByDescending(bt => bt.NgayTao)
            .ProjectTo<BaiTapDto>(_mapper.ConfigurationProvider);

        var result = await query.PaginatedListAsync(request.PageNumber, request.PageSize);

        return new Output
        {
            isError = false,
            data = result,
            code = 200,
            message = "Lấy danh sách bài tập của học sinh thành công"
        };
    }
}

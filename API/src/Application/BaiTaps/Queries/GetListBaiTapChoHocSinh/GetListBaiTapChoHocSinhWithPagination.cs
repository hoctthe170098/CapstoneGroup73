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
    private readonly IIdentityService _identityService;

    public TeacherAssignmentListWithPaginationQueryHandler(
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

    public async Task<Output> Handle(TeacherAssignmentListWithPaginationQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
            throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(hs => hs.UserId == userId, cancellationToken);

        if (hocSinh == null)
            throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var hocSinhCode = hocSinh.Code;

        var query = _context.BaiTaps
            .AsNoTracking()
            .Where(bt => bt.LichHoc.ThamGiaLopHocs.Any(tg => tg.HocSinhCode == hocSinhCode))
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

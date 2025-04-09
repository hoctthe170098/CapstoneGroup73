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
    public string? TrangThai { get; init; }
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
            .FirstOrDefaultAsync(hs => hs.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var hocSinhCode = hocSinh.Code;

        // Lấy tất cả lớp học học sinh đang tham gia
        var allClassesQuery = _context.ThamGiaLopHocs
            .Where(tg => tg.HocSinhCode == hocSinhCode)
            .Select(tg => tg.LichHoc.TenLop)
            .Distinct();

        var allClasses = await allClassesQuery.ToListAsync(cancellationToken);

        var totalCount = allClasses.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedClasses = allClasses
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var groupedResults = new List<BaiTapGroupByTenLopDto>();

        foreach (var tenLop in paginatedClasses)
        {
            var baiTapQuery = _context.BaiTaps
                .AsNoTracking()
                .Where(bt =>
                    bt.LichHoc.TenLop == tenLop &&
                    bt.LichHoc.ThamGiaLopHocs.Any(tg => tg.HocSinhCode == hocSinhCode));

            //  Filter theo trạng thái (nếu không phải "all")
            if (!string.IsNullOrWhiteSpace(request.TrangThai) && request.TrangThai.ToLower() != "all")
            {
                baiTapQuery = baiTapQuery.Where(bt => bt.TrangThai == request.TrangThai);
            }

            var baiTaps = await baiTapQuery
                .OrderByDescending(bt => bt.NgayTao)
                .ProjectTo<BaiTapDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            groupedResults.Add(new BaiTapGroupByTenLopDto
            {
                TenLop = tenLop,
                BaiTaps = baiTaps
            });
        }

        var result = new BaiTapWithPaginationDTO
        {
            PageNumber = request.PageNumber,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = groupedResults
        };

        return new Output
        {
            isError = false,
            data = result,
            code = 200,
            message = "Lấy danh sách bài tập của học sinh thành công"
        };
    }
}

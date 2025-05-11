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
    public string? TenLop { get; init; }
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

        if (string.IsNullOrWhiteSpace(request.TenLop))
            throw new WrongInputException("Tên lớp không được bỏ trống!");

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            throw new Exception("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(hs => hs.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var hocSinhCode = hocSinh.Code;
        var tenLop = request.TenLop.Trim();

        // Kiểm tra học sinh có tham gia lớp này không
        var isInClass = await _context.ThamGiaLopHocs
            .AnyAsync(tg => tg.HocSinhCode == hocSinhCode 
            && tg.LichHoc.TenLop == tenLop&&tg.LichHoc.Phong.CoSoId==coSoId, cancellationToken);

        if (!isInClass)
            throw new NotFoundIDException();

        // CẬP NHẬT TRẠNG THÁI CỦA NHỮNG BÀI TẬP HẾT HẠN TRƯỚC KHI TRUY VẤN
        var now = DateTime.Now;
        var expiredAssignments = await _context.BaiTaps
            .Where(bt => bt.LichHoc.TenLop == tenLop &&
                         bt.LichHoc.Phong.CoSoId==coSoId &&
                         bt.ThoiGianKetThuc.HasValue &&
                         bt.ThoiGianKetThuc.Value <= now &&
                         bt.TrangThai != "Kết thúc")
            .ToListAsync(cancellationToken);
        foreach (var baiTap in expiredAssignments)
        {
            baiTap.TrangThai = "Kết thúc";
        }

        if (expiredAssignments.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // TRUY VẤN DANH SÁCH BÀI TẬP
        var baiTapQuery = _context.BaiTaps
            .AsNoTracking()
            .Where(bt =>
                bt.LichHoc.TenLop == tenLop &&
                bt.LichHoc.Phong.CoSoId == coSoId &&
                bt.LichHoc.ThamGiaLopHocs
                .Any(tg => tg.HocSinhCode == hocSinhCode&&tg.NgayKetThuc >= DateOnly.FromDateTime(bt.NgayTao)));

        if (!string.IsNullOrWhiteSpace(request.TrangThai) && request.TrangThai.ToLower() != "all")
        {
            baiTapQuery = baiTapQuery.Where(bt => bt.TrangThai == request.TrangThai);
        }

        var totalCount = await baiTapQuery.CountAsync(cancellationToken);

        var baiTaps = await baiTapQuery
            .OrderByDescending(bt => bt.NgayTao)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<BaiTapDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var result = new BaiTapWithPaginationDTO
        {
            PageNumber = request.PageNumber,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            Items = new List<BaiTapGroupByTenLopDto>
            {
                new BaiTapGroupByTenLopDto
                {
                    TenLop = tenLop,
                    BaiTaps = baiTaps
                }
            }
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy danh sách bài tập theo lớp thành công"
        };
    }
}



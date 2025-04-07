using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;

public record GetListBaiTapChoGiaoVienWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public required string TenLop { get; init; }
    public string? TrangThai { get; init; } = "all";
    public DateTime? ThoiGianKetThuc { get; init; } = DateTime.MinValue;
}

public class GetListBaiTapChoGiaoVienWithPaginationQueryHandler
    : IRequestHandler<GetListBaiTapChoGiaoVienWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetListBaiTapChoGiaoVienWithPaginationQueryHandler(
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

    public async Task<Output> Handle(GetListBaiTapChoGiaoVienWithPaginationQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
            throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var giaoVien = await _context.GiaoViens
            .AsNoTracking()
            .FirstOrDefaultAsync(gv => gv.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy giáo viên tương ứng với tài khoản.");

        var giaoVienCode = giaoVien.Code;

        // Lọc lớp học theo giáo viên và tên lớp nếu có
        var queryLop = _context.LichHocs
            .Where(lh => lh.GiaoVienCode == giaoVienCode)
            .Select(lh => lh.TenLop)
            .Distinct();

        if (!string.IsNullOrWhiteSpace(request.TenLop))
        {
            queryLop = queryLop.Where(l => l == request.TenLop.Trim());
        }

        var listClass = await queryLop.ToListAsync(cancellationToken);

        var totalCount = listClass.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedClasses = listClass
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thu = (int)DateTime.UtcNow.DayOfWeek;
        thu = thu == 0 ? 8 : thu + 1;

        var baiTapGroupList = new List<BaiTapGroupByTenLopDto>();

        foreach (var tenLop in paginatedClasses)
        {
            IQueryable<BaiTap> baiTapQuery;

            var lichHoc = await _context.LichHocs
                .AsNoTracking()
                .FirstOrDefaultAsync(lh =>
                    lh.GiaoVienCode == giaoVienCode &&
                    lh.TenLop == tenLop &&
                    lh.Thu == thu &&
                    lh.NgayBatDau <= today && today <= lh.NgayKetThuc,
                    cancellationToken);

            if (lichHoc?.TrangThai == "Dạy thay")
            {
                baiTapQuery = _context.BaiTaps.Where(bt => bt.LichHocId == lichHoc.Id);
            }
            else
            {
                baiTapQuery = _context.BaiTaps
                    .Include(bt => bt.LichHoc)
                    .Where(bt =>
                        bt.LichHoc.TenLop == tenLop &&
                        bt.LichHoc.GiaoVienCode == giaoVienCode);
            }

            // Áp dụng filter theo trạng thái
            if (!string.IsNullOrWhiteSpace(request.TrangThai))
            {
                baiTapQuery = baiTapQuery.Where(bt =>
                    bt.TrangThai == request.TrangThai);
            }

            // Áp dụng filter theo ngày kết thúc (bỏ qua phần giờ phút)
            if (request.ThoiGianKetThuc.HasValue)
            {
                var ngayFilter = request.ThoiGianKetThuc.Value.Date;

                baiTapQuery = baiTapQuery.Where(bt =>
                    bt.ThoiGianKetThuc.HasValue &&
                    bt.ThoiGianKetThuc.Value.Date == ngayFilter);
            }
            var baiTaps = await baiTapQuery
                .OrderByDescending(bt => bt.NgayTao)
                .ProjectTo<BaiTapGiaoVienDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            baiTapGroupList.Add(new BaiTapGroupByTenLopDto
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
            Items = baiTapGroupList
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy danh sách bài tập thành công"
        };
    }
}

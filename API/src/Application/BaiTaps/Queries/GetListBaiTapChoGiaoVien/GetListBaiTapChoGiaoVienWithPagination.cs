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
    public string? TrangThai { get; init; }
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
            throw new Exception("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var giaoVien = await _context.GiaoViens
            .AsNoTracking()
            .FirstOrDefaultAsync(gv => gv.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy giáo viên tương ứng với tài khoản.");

        var giaoVienCode = giaoVien.Code;

        var baiTapQuery = _context.BaiTaps
            .Include(bt => bt.LichHoc)
            .Where(bt =>
                bt.LichHoc.GiaoVienCode == giaoVienCode &&
                bt.LichHoc.TenLop == request.TenLop);

        // Lọc theo trạng thái nếu có
        if (!string.IsNullOrWhiteSpace(request.TrangThai) && request.TrangThai.ToLower() != "all")
        {
            baiTapQuery = baiTapQuery.Where(bt => bt.TrangThai == request.TrangThai);
        }

        // Cập nhật trạng thái nếu bài tập hết hạn
        var now = DateTime.Now;
        var expiredAssignments = await baiTapQuery
            .Where(bt =>
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

        var totalCount = await baiTapQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var baiTaps = await baiTapQuery
            .OrderByDescending(bt => bt.NgayTao)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<BaiTapGiaoVienDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var result = new BaiTapWithPaginationDTO
        {
            PageNumber = request.PageNumber,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = new List<BaiTapGroupByTenLopDto>
            {
                new BaiTapGroupByTenLopDto
                {
                    TenLop = request.TenLop,
                    BaiTaps = baiTaps
                }
            }
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

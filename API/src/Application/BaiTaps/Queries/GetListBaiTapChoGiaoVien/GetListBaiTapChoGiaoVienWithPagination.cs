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

        //  Tìm giáo viên từ userId
        var giaoVien = await _context.GiaoViens
            .AsNoTracking()
            .FirstOrDefaultAsync(gv => gv.UserId == userId, cancellationToken);

        if (giaoVien == null)
            throw new NotFoundDataException("Không tìm thấy giáo viên tương ứng với tài khoản.");

        var giaoVienCode = giaoVien.Code;
        var tenLop = request.TenLop;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thu = (int)DateTime.UtcNow.DayOfWeek;
        thu = thu == 0 ? 8 : thu + 1;

        //  Tìm lịch học hôm nay của lớp đó theo giáo viên
        var lichHoc = await _context.LichHocs
            .AsNoTracking()
            .FirstOrDefaultAsync(lh =>
                lh.GiaoVienCode == giaoVienCode &&
                lh.TenLop == tenLop &&
                lh.Thu == thu &&
                lh.NgayBatDau <= today && today <= lh.NgayKetThuc,
                cancellationToken);

        if (lichHoc == null)
            throw new NotFoundIDException();

        IQueryable<BaiTap> baiTapQuery;

        if (lichHoc.TrangThai == "Dạy thay")
        {
            //  Chỉ lấy bài tập thuộc lịch học dạy thay hôm nay
            baiTapQuery = _context.BaiTaps
                .Where(bt => bt.LichHocId == lichHoc.Id);
        }
        else
        {
            //  Lấy tất cả bài tập thuộc lớp của giáo viên đó
            baiTapQuery = _context.BaiTaps
                .Include(bt => bt.LichHoc)
                .Where(bt =>
                    bt.LichHoc.TenLop == tenLop &&
                    bt.LichHoc.GiaoVienCode == giaoVienCode);
        }

        var result = await baiTapQuery
            .AsNoTracking()
            .OrderByDescending(bt => bt.NgayTao)
            .Select(bt => new
            {
                bt.Id,
                bt.TieuDe,
                bt.NoiDung,
                bt.NgayTao,
                bt.ThoiGianKetThuc,
                bt.TrangThai,
                bt.UrlFile
            })
            .ToListAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = new
            {
                TenLop = tenLop,
                BaiTaps = result
            },
            message = "Lấy danh sách bài tập thành công"
        };
    }
}

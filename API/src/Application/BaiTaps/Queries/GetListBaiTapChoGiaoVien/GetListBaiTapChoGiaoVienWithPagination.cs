using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using System.Security.Claims;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;

public record GetListBaiTapChoGiaoVienWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetListBaiTapChoGiaoVienWithPaginationQueryHandler: IRequestHandler<GetListBaiTapChoGiaoVienWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetListBaiTapChoGiaoVienWithPaginationQueryHandler(IApplicationDbContext context,IMapper mapper,IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetListBaiTapChoGiaoVienWithPaginationQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
            throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");

        var user = _httpContextAccessor.HttpContext?.User;
        var giaoVienCode = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user?.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(giaoVienCode))
            throw new UnauthorizedAccessException("Không xác định được giáo viên từ token.");

        var query = _context.BaiTaps
            .AsNoTracking()
            .Where(bt => bt.LichHoc.GiaoVienCode == giaoVienCode)
            .OrderByDescending(bt => bt.NgayTao)
            .ProjectTo<BaiTapGiaoVienDto>(_mapper.ConfigurationProvider);

        var result = await query.PaginatedListAsync(request.PageNumber, request.PageSize);

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy danh sách bài tập của giáo viên thành công"
        };
    }
}

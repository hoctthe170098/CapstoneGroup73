using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;

public record GetListBaiTapChoGiaoVienWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetListBaiTapChoGiaoVienWithPaginationQueryHandler : IRequestHandler<GetListBaiTapChoGiaoVienWithPaginationQuery, Output>
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

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var giaoVienId = _identityService.GetUserId(token); 
        var giaoVienIdStr = giaoVienId.ToString();

        var query = _context.BaiTaps
            .AsNoTracking()
            .Where(bt => bt.LichHoc.GiaoVien.UserId == giaoVienIdStr)
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

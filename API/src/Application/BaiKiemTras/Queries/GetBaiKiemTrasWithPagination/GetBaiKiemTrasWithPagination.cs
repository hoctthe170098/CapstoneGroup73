using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;

namespace StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
public record GetBaiKiemTrasWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 3;
    public required string TrangThai { get; init; } = "all";
    public required string TenLop { get; init; } = "all";
}
public class GetBaiKiemTrasWithPaginationQueryHandler : IRequestHandler<GetBaiKiemTrasWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaiKiemTrasWithPaginationQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetBaiKiemTrasWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var listTest = await _context.BaiKiemTras.ToListAsync(cancellationToken);
        var query =  listTest.AsQueryable();
        if (request.TrangThai.ToLower().Trim() != "all")
        {
            query = query.Where(q=>q.TrangThai.ToLower().Trim()==request.TrangThai.ToLower().Trim());
        }
        if (request.TenLop.ToLower().Trim() != "all")
        {
            query = query.Where(q => q.LichHoc.TenLop == request.TrangThai.ToLower().Trim());
        }
        var list = query.ToList();
        list = list
            .Skip((request.PageNumber-1)*request.PageSize)
            .Take(request.PageSize)
            .ToList();
        return new Output
        {
            code = 200,
            isError = false,
            message = "Lấy danh sách bài kiểm tra thành công",
            data = new 
            {
                PageNumber = request.PageNumber,
                TotalCount = query.Count(),
                TotalPages = (query.Count()%request.PageSize==0)
                ?query.Count()/request.PageSize
                :query.Count()/request.PageSize + 1,
                data = list
            }
        };
    }
}

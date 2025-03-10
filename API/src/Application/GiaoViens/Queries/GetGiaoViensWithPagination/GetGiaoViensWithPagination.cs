using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination.GetGiaoViensWithPagination;
using StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;

namespace StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;
public class GetGiaoViensWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTen { get; init; }
    public string? FilterTenCoSo { get; init; }
    public string? FilterGioiTinh { get; init; }
    public string? SortBy { get; init; }
}

public class GetGiaoViensWithPaginationQueryHandler
     : IRequestHandler<GetGiaoViensWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGiaoViensWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetGiaoViensWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1) throw new WrongInputException();
            var query = _context.GiaoViens.Include(nv => nv.Coso).AsQueryable();

            // Search by "Ten"
            if (!string.IsNullOrWhiteSpace(request.SearchTen))
            {
                query = query.Where(nv => nv.Ten.Contains(request.SearchTen));
            }

            // Filter by "TenCoSo"
            if (!string.IsNullOrWhiteSpace(request.FilterTenCoSo))
            {
                query = query.Where(nv => nv.Coso.Ten == request.FilterTenCoSo);
            }

            // Filter by "GioiTinh"
            if (!string.IsNullOrWhiteSpace(request.FilterGioiTinh))
            {
                query = query.Where(nv => nv.GioiTinh == request.FilterGioiTinh);
            }

            // Sorting
            query = request.SortBy switch
            {
                "Code" => query.OrderBy(nv => nv.Code),
                "Ten" => query.OrderBy(nv => nv.Ten),
                _ => query.OrderBy(nv => nv.Code) 
            };

            var list = await query
               .ProjectTo<GiaoVienDto>(_mapper.ConfigurationProvider)
               .PaginatedListAsync(request.PageNumber, request.PageSize);

            return new Output
            {
                isError = false,
                data = list,
                code = 200
            };
        }
        catch
        {
            throw new WrongInputException();
        }
    }
}

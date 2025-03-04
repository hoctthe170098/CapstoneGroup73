using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Mappings;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;

public record GetChuongTrinhsWithPaginationQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<ChuongTrinhDto>>;

public class GetChuongTrinhsWithPaginationQueryHandler : IRequestHandler<GetChuongTrinhsWithPaginationQuery, PaginatedList<ChuongTrinhDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetChuongTrinhsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ChuongTrinhDto>> Handle(GetChuongTrinhsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.ChuongTrinhs
            .OrderBy(ct => ct.Id)
            .ProjectTo<ChuongTrinhDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

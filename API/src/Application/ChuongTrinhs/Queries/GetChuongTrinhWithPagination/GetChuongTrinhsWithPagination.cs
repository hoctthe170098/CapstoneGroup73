using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Exceptions;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;

public record GetChuongTrinhsWithPaginationQuery: IRequest<Output>
{
    public string? Search { get; init; } = "";
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 2;
}

public class GetChuongTrinhsWithPaginationQueryHandler : IRequestHandler<GetChuongTrinhsWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetChuongTrinhsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetChuongTrinhsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            string noidungSearch = (string.IsNullOrWhiteSpace(request.Search)) ? "" : request.Search;
            if (request.PageNumber < 1 || request.PageSize < 1) throw new WrongInputException();
            var list = await _context.ChuongTrinhs
                .Where(c => (c.TieuDe.Contains(noidungSearch) || c.MoTa.Contains(noidungSearch))&&c.TrangThai=="use")
                   .ProjectTo<ChuongTrinhDto>(_mapper.ConfigurationProvider)
                   .PaginatedListAsync(request.PageNumber, request.PageSize);
            return new Output
            {
                isError = false,
                data = list,
                code = 200,
                message = "Lấy danh sách chương trình thành công"
            };
        }
        catch
        {
            throw ;
        }         
    }
}

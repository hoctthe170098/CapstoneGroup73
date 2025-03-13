using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
public record  GetCososWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 6;
}
public class GetCososWithPaginationQueryHandler 
    : IRequestHandler<GetCososWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetCososWithPaginationQueryHandler(IApplicationDbContext context
        , IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetCososWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1) throw new WrongInputException();
            var list = await _context.CoSos.ProjectTo<CoSoDto>(_mapper.ConfigurationProvider)
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


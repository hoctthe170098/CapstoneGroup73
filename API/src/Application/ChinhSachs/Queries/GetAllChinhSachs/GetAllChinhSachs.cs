using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StudyFlow.Application.ChinhSachs.Queries.GetAllChinhSachs;

public record GetAllChinhSachsQuery : IRequest<Output>;

public class GetAllChinhSachsQueryHandler : IRequestHandler<GetAllChinhSachsQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllChinhSachsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetAllChinhSachsQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.ChinhSaches
            .AsNoTracking()
            .ProjectTo<ChinhSachDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = list,
            code = 200
        };
    }
}

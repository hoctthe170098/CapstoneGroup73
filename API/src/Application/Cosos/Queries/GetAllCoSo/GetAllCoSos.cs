using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.TodoLists.Queries.GetTodos;
using StudyFlow.Domain.Enums;

namespace StudyFlow.Application.Cosos.Queries.GetAllCoSo;
public record GetAllCoSosQuery : IRequest<Output>
{

}
public class GetAllCoSosQueryHandler : IRequestHandler<GetAllCoSosQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllCoSosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetAllCoSosQuery request, CancellationToken cancellationToken)
    {
        return new Output
        {
            isError = false,
            code=200,
            data = await _context.CoSos
                .ProjectTo<GetAllCoSoDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken),
            message = "Lấy danh sách cơ sở thành công"
        };
    }
}

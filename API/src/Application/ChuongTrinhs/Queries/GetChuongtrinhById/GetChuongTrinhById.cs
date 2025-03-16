using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Exceptions;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhById;

public record GetChuongTrinhByIdQuery: IRequest<Output>
{
    public required int chuongTrinhId { get; init; }
}

public class GetChuongTrinhByIdQueryHandler : IRequestHandler<GetChuongTrinhByIdQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetChuongTrinhByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetChuongTrinhByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.ChuongTrinhs
                .ProjectTo<ChuongTrinhDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(c=>c.Id==request.chuongTrinhId);
            if(entity==null) throw new NotFoundIDException();
            return new Output
            {
                isError = false,
                data = entity,
                code = 200,
                message = "Lấy chương trình thành công"
            };
        }
        catch
        {
            throw;
        }         
    }
}

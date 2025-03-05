using StudyFlow.Application.Common.Interfaces;

namespace StudyFlow.Application.Slots.Queries.GetSlots;

public record GetSlotsQuery : IRequest<List<SlotDto>>;

public class GetSlotsQueryHandler : IRequestHandler<GetSlotsQuery, List<SlotDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSlotsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SlotDto>> Handle(GetSlotsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Slots
            .AsNoTracking()
            .ProjectTo<SlotDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}

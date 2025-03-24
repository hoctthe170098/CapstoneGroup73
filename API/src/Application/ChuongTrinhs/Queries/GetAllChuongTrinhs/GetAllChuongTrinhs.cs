using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Queries.GetAllCoSo;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetAllChuongTrinhs;
public record GetAllChuongTrinhsQuery : IRequest<Output>
{

}
public class GetAllChuongTrinhsQueryHandler : IRequestHandler<GetAllChuongTrinhsQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllChuongTrinhsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetAllChuongTrinhsQuery request, CancellationToken cancellationToken)
    {
        return new Output
        {
            isError = false,
            code = 200,
            data = await _context.ChuongTrinhs
            .Where(ct=>ct.TrangThai=="use")
                .ProjectTo<ChuongTrinhDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken),
            message = "Lấy danh sách chương trình thành công"
        };
    }
}

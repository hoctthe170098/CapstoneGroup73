using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.TraLois.Queries.GetBaoCaoNhanXetHocSinh;
public class GetBaoCaoNhanXetHocSinhQuery : IRequest<Output>
{
    public required string HocSinhCode { get; init; }
}
public class GetBaoCaoNhanXetHocSinhQueryHandler : IRequestHandler<GetBaoCaoNhanXetHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;

    public GetBaoCaoNhanXetHocSinhQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(GetBaoCaoNhanXetHocSinhQuery request, CancellationToken cancellationToken)
    {
        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(hs => hs.Code == request.HocSinhCode, cancellationToken);

        if (hocSinh == null)
            throw new NotFoundDataException("Không tìm thấy học sinh.");

        var lichSuNhanXet = await _context.TraLois
            .Where(t => t.HocSinhCode == request.HocSinhCode)
            .OrderByDescending(t => t.ThoiGian)
            .Select((t, index) => new
            {
                STT = index + 1,
                Ngay = t.ThoiGian.ToString("dd/MM/yyyy"),
                NhanXet = t.NhanXet
            })
            .ToListAsync(cancellationToken);

        var result = new
        {
            HocSinh = new
            {
                hocSinh.Ten,
                hocSinh.Code
            },
            LichSu = lichSuNhanXet
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy báo cáo nhận xét học sinh thành công."
        };
    }
}

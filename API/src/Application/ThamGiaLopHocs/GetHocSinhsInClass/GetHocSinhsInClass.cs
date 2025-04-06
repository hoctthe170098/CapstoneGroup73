using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ThamGiaLopHocs.GetHocSinhsInClass;
public record GetHocSinhsInClassQueries : IRequest<Output>
{
    public required string TenLop { get; init; }
}

public class GetHocSinhsInClassQueriesHandler : IRequestHandler<GetHocSinhsInClassQueries, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetHocSinhsInClassQueriesHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetHocSinhsInClassQueries request, CancellationToken cancellationToken)
    {
        try
        {
            var list = await _context.ThamGiaLopHocs
                .Include(t => t.LichHoc)
                .Where(t => t.LichHoc.TenLop == request.TenLop)
                .Select(t => t.HocSinh)
                .OrderByDescending(hs => hs.Ten)
                .ToListAsync(cancellationToken);

            return new Output
            {
                isError = false,
                data = list,
                message = "Lấy danh sách học sinh thành công"
            };
        }
        catch
        {
            throw new NotImplementedException();
        }
    }
}

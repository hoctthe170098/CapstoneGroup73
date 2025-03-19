using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
public record GetLopHocWithPaginationQuery : IRequest<Output>
{
    public required string TenLop {  get; init; }
    public required List<int> Thus { get; init; }
    public required string GiaoVienCode { get; init; }
    public required int PhongId {  get; init; }
    public required int ChuongTrinhId { get; init; }
    public required string TrangThai {  get; init; }
    public required TimeOnly ThoiGianBatDau {  get; init; }
    public required TimeOnly ThoiGianKetThuc { get; init; }
    public required DateOnly NgayBatDau { get; init; }
    public required DateOnly NgayKetThuc { get; init; }

}
public class GetLopHocWithPaginationQueryHandler : IRequestHandler<GetLopHocWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetLopHocWithPaginationQueryHandler(IApplicationDbContext context
        ,IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public Task<Output> Handle(GetLopHocWithPaginationQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

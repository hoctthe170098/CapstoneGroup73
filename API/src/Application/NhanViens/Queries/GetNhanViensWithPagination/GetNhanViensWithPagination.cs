using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;
public record GetNhanViensWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTen { get; init; } 
    public string? FilterTenCoSo { get; init; } 
    public string? FilterTenVaiTro { get; init; } 
    public string? SortBy { get; init; }
    public bool? IsActive { get; init; }
}

public class GetNhanViensWithPaginationQueryHandler
    : IRequestHandler<GetNhanViensWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public GetNhanViensWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetNhanViensWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1) throw new WrongInputException();
            var query = _context.NhanViens.Include(nv => nv.Coso).AsQueryable();

            // Search by "Ten"
            if (!string.IsNullOrWhiteSpace(request.SearchTen))
            {
                query = query.Where(nv => nv.Ten.Contains(request.SearchTen)
                ||nv.Code.Contains(request.SearchTen));
            }

            // Filter by "TenCoSo"
            if (!string.IsNullOrWhiteSpace(request.FilterTenCoSo))
            {
                var checkFormatCoSo=Guid.TryParse(request.FilterTenCoSo, out var coSo_id);
                if(checkFormatCoSo) 
                query = query.Where(nv => nv.Coso.Id == coSo_id);
            }
            // Filter by Status
            if (request.IsActive.HasValue)
            {
                var userIds = await _context.NhanViens
                    .Where(nv => nv.UserId != null)
                    .Select(nv => nv.UserId!)
                    .Distinct()
                    .ToListAsync();

                var filteredUserIds = new List<string>();

                foreach (var userId in userIds)
                {
                    var isActive = await _identityService.IsUserActiveAsync(userId);
                    if (isActive == request.IsActive.Value)
                    {
                        filteredUserIds.Add(userId);
                    }
                }
                query = query.Where(nv => nv.UserId != null && filteredUserIds.Contains(nv.UserId!));
            }

            // Filter by "TenVaiTro"
            if (!string.IsNullOrWhiteSpace(request.FilterTenVaiTro))
            {
                var userIdsWithRole = new List<string>();

                var userIds = await _context.NhanViens
                    .Where(nv => nv.UserId != null)
                    .Select(nv => nv.UserId!)
                    .Distinct()
                    .ToListAsync();

                foreach (var userId in userIds)
                {
                    var roles = await _identityService.GetRolesByUserId(userId);
                    if (roles.Contains(request.FilterTenVaiTro))
                    {
                        userIdsWithRole.Add(userId);
                    }
                }

                query = query.Where(nv => nv.UserId != null && userIdsWithRole.Contains(nv.UserId!));
            }

            // Sorting
            query = request.SortBy switch
            {
                "Code" => query.OrderBy(nv => nv.Code),
                "Ten" => query.OrderBy(nv => nv.Ten),
                _ => query.OrderBy(nv => nv.Code) // Default sorting by Code
            };

            var list = await query
               .ProjectTo<NhanVienDto>(_mapper.ConfigurationProvider)
               .PaginatedListAsync(request.PageNumber, request.PageSize);

            // Fetch Role Names Using IIdentityService
            foreach (var dto in list.Items) 
            {
                var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.Code == dto.Code);
                if (nhanVien != null && !string.IsNullOrEmpty(nhanVien.UserId))
                {
                    var roles = await _identityService
                        .GetRolesByUserId(nhanVien.UserId);
                    dto.TenVaiTro = roles.FirstOrDefault();
                    dto.TrangThai = await _identityService
                        .IsUserActiveAsync(nhanVien.UserId);
                }
            }

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

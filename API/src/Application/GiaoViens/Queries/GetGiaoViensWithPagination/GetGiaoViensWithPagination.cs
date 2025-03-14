using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination.GetGiaoViensWithPagination;
using StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;

namespace StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;
public class GetGiaoViensWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTen { get; init; }
    public string? SortBy { get; init; }
    public bool? IsActive { get; init; }
}

public class GetGiaoViensWithPaginationQueryHandler
     : IRequestHandler<GetGiaoViensWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetGiaoViensWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetGiaoViensWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1) throw new WrongInputException();

            // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
            var coSoId = _identityService.GetCampusId(token);

            var query = _context.GiaoViens
                .Include(nv => nv.Coso)
                .Include(nv => nv.LicHocs)
                .Where(gv => gv.CoSoId == coSoId)
                .AsQueryable();

            // Search by "Ten"
            if (!string.IsNullOrWhiteSpace(request.SearchTen))
            {
                string nameLower = request.SearchTen.ToLower();
                query = query.Where(nv => nv.Ten.Contains(nameLower));
            }

            // Filter by Status
            if(request.IsActive.HasValue)
            {
                var userIds = await _context.GiaoViens
                    .Where(gv => gv.UserId != null)
                    .Select(gv => gv.UserId!)
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

                query = query.Where(gv => gv.UserId != null && filteredUserIds.Contains(gv.UserId!));
            }

            // Sorting
            query = request.SortBy switch
            {
                "Code" => query.OrderBy(nv => nv.Code),
                "Ten" => query.OrderBy(nv => nv.Ten),
                _ => query.OrderBy(nv => nv.Code) 
            };

            var list = await query
               .ProjectTo<GiaoVienDto>(_mapper.ConfigurationProvider)
               .ToListAsync();

            foreach (var giaoVienDto in list)
            {
                if (!string.IsNullOrEmpty(giaoVienDto.Code))
                {
                    var user = await _context.GiaoViens
                        .Where(gv => gv.Code == giaoVienDto.Code)
                        .Select(gv => gv.UserId)
                        .FirstOrDefaultAsync();

                    giaoVienDto.IsActive = user != null && await _identityService.IsUserActiveAsync(user);
                }
            }

            var paginatedList = list
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

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

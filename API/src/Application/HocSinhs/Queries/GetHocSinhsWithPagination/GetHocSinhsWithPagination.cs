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

namespace StudyFlow.Application.HocSinhs.Queries.GetHocViensWithPagination;
public record GetHocSinhsWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTen { get; init; }
    public string? SortBy { get; init; }
    public bool? FilterIsActive { get; init; } 
    public string? FilterByClass { get; init; }
}

public class GetHocSinhsWithPaginationQueryHandler
    : IRequestHandler<GetHocSinhsWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetHocSinhsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetHocSinhsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1) throw new WrongInputException();

            // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
            var coSoId = _identityService.GetCampusId(token);

            var query = _context.HocSinhs
                .Include(hs => hs.Coso)
                .Include(hs => hs.ChinhSach)
                .Where(hs => hs.CoSoId == coSoId)
                .AsQueryable();

            // Search name
            if (!string.IsNullOrWhiteSpace(request.SearchTen))
            {
                query = query.Where(hs => hs.Ten.ToLower().Contains(request.SearchTen.ToLower()));
            }

            // Sort by name
            query = request.SortBy switch
            {
                "Code" => query.OrderBy(hs => hs.Code),
                "Ten" => query.OrderBy(hs => hs.Ten),
                _ => query.OrderBy(hs => hs.Code)
            };

            var list = await query
               .ProjectTo<HocSinhDto>(_mapper.ConfigurationProvider)
               .ToListAsync();

            // Fetch class enrolled list of student
            var hocSinhCodes = list.Select(hs => hs.Code).ToList();
            var thamGiaLopHocs = await _context.ThamGiaLopHocs
                .Where(tg => hocSinhCodes.Contains(tg.HocSinhCode))
                .Include(tg => tg.LichHoc)
                .ToListAsync();

            // Assign student active status and class enrolled
            foreach (var hocSinhDto in list)
            {
                var tenLops = thamGiaLopHocs
                    .Where(tg => tg.HocSinhCode == hocSinhDto.Code)
                    .Select(tg => tg.LichHoc.TenLop)
                    .Distinct()
                    .ToList();

                hocSinhDto.TenLops = tenLops;

                if (!string.IsNullOrEmpty(hocSinhDto.UserId))
                {
                    hocSinhDto.IsActive = await _identityService.IsUserActiveAsync(hocSinhDto.UserId);
                }
            }

            // Pagination
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

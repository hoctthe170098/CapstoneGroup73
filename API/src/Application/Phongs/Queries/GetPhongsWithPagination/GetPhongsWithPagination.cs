using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using System.Security.Claims;

namespace StudyFlow.Application.Phongs.Queries.GetPhongsWithPagination;

public record GetPhongsWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetPhongsWithPaginationQueryHandler : IRequestHandler<GetPhongsWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetPhongsWithPaginationQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetPhongsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");

            // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

            // Lấy CoSoId từ JWT token của người dùng (Fix: use await)
            var coSoId =  _identityService.GetCampusId(token);
           

            // Chỉ lấy danh sách phòng thuộc cơ sở của Campus Manager
            var query = _context.Phongs
                .AsNoTracking()
                .Where(p => p.CoSoId == coSoId);

            var list = await query
                .ProjectTo<PhongDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return new Output
            {
                isError = false,
                data = list,
                code = 200
            };
        }
        catch (Exception ex)
        {
            throw new WrongInputException($"Có lỗi xảy ra khi lấy danh sách phòng: {ex.Message}");
        }
    }
}

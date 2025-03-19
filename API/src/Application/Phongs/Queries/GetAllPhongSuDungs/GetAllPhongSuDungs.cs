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

namespace StudyFlow.Application.Phongs.Queries.GetAllPhongSuDungs;

public record GetAllPhongSuDungsQuery : IRequest<Output>;
public class GetAllsPhongSuDungsQueryHandler : IRequestHandler<GetAllPhongSuDungsQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllsPhongSuDungsQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(GetAllPhongSuDungsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

            // Lấy CoSoId từ JWT token của người dùng
            var coSoId = _identityService.GetCampusId(token);

            // Chỉ lấy danh sách phòng thuộc cơ sở của Campus Manager
            var list = await _context.Phongs
                .AsNoTracking()
                .Where(p => p.CoSoId == coSoId&&p.TrangThai=="use")
                .ProjectTo<PhongDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new Output
            {
                isError = false,
                data = list,
                code = 200
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Có lỗi xảy ra khi lấy danh sách phòng: {ex.Message}");
        }
    }
}

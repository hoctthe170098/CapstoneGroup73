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
using StudyFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.HocSinhs.Queries.GetHocSinhsByNameOrCode;
public class GetHocSinhsByNameOrCodeQuery : IRequest<Output>
{
    public string? SearchTen { get; init; }
}

public class GetHocSinhsByNameOrCodeQueryHandler
    : IRequestHandler<GetHocSinhsByNameOrCodeQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetHocSinhsByNameOrCodeQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Output> Handle(GetHocSinhsByNameOrCodeQuery request, CancellationToken cancellationToken)
    {
            // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
            var coSoId = _identityService.GetCampusId(token);
            // Lấy danh sách HocSinh từ cơ sở dữ liệu
            var hocSinhs = await _context.HocSinhs
                .Where(gv => gv.CoSoId == coSoId && gv.UserId != null)
                .ToListAsync(cancellationToken);
            // Lọc theo tên
            if (!string.IsNullOrWhiteSpace(request.SearchTen))
            {
                string nameLower = request.SearchTen.ToLower();
                hocSinhs = hocSinhs.Where(gv => gv.Ten.ToLower().Contains(nameLower) || gv.Code.Contains(request.SearchTen)).ToList();
            }
            // Lọc theo trạng thái hoạt động của người dùng
            var filteredHocSinhs = new List<HocSinh>();
            foreach (var gv in hocSinhs)
            {
                if (await _identityService.IsUserActiveAsync(gv.UserId!))
                {
                    filteredHocSinhs.Add(gv);
                }
            }
            // Ánh xạ sang HocSinhDto
            var list = _mapper.Map<List<HocSinhDto>>(filteredHocSinhs);
            return new Output
            {
                isError = false,
                data = list,
                code = 200
            };
        }
    }

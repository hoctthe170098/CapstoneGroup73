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
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensByNameOrCode;
using StudyFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.GiaoViens.Queries.GetGiaoViensByNameOrCode;
public class GetGiaoViensByNameOrCodeQuery : IRequest<Output>
{
    public string? SearchTen { get; init; }
}

public class GetGiaoViensByNameOrCodeQueryHandler
    : IRequestHandler<GetGiaoViensByNameOrCodeQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetGiaoViensByNameOrCodeQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Output> Handle(GetGiaoViensByNameOrCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
            var coSoId = _identityService.GetCampusId(token);
            // Lấy danh sách GiaoVien từ cơ sở dữ liệu
            var giaoViens = await _context.GiaoViens
                .Where(gv => gv.CoSoId == coSoId && gv.UserId != null)
                .ToListAsync(cancellationToken);
            // Lọc theo tên
            if (!string.IsNullOrWhiteSpace(request.SearchTen))
            {
                string nameLower = request.SearchTen.ToLower();
                giaoViens = giaoViens.Where(gv => gv.Ten.ToLower().Contains(nameLower) || gv.Code.Contains(request.SearchTen)).ToList();
            }
            // Lọc theo trạng thái hoạt động của người dùng
            var filteredGiaoViens = new List<GiaoVien>();
            foreach (var gv in giaoViens)
            {
                if (await _identityService.IsUserActiveAsync(gv.UserId!))
                {
                    filteredGiaoViens.Add(gv);
                }
            }
            // Ánh xạ sang GiaoVienDto
            var list = _mapper.Map<List<GiaoVienDto>>(filteredGiaoViens);
            return new Output
            {
                isError = false,
                data = list,
                code = 200
            };
        }
        catch (Exception)
        {
            throw new WrongInputException();
        }
    }
}

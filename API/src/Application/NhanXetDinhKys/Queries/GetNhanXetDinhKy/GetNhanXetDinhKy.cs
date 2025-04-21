using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using StudyFlow.Domain.Enums;

namespace StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
public record GetNhanXetDinhKysQuery : IRequest<Output>
{
    public required string TenLop {  get; set; }
    public required string HocSinhCode {  get; set; }
}
public class GetNhanXetDinhKysQueryHandler : IRequestHandler<GetNhanXetDinhKysQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetNhanXetDinhKysQueryHandler(IApplicationDbContext context, IMapper mapper
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetNhanXetDinhKysQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var ThamGiaLopHocs = await _context.ThamGiaLopHocs
            .Where(tg=>tg.HocSinhCode==request.HocSinhCode
            &&tg.LichHoc.TenLop==request.TenLop
            &&tg.LichHoc.Phong.CoSoId==coSoId)
            .Include(tg=>tg.LichHoc)
            .Include(tg=>tg.HocSinh)
            .ToListAsync();
        var NgayDaHoc = await _context.DiemDanhs
            .Where(dd => ThamGiaLopHocs.Select(tg => tg.Id).Contains(dd.ThamGiaLopHocId))
            .Select(dd=>dd.Ngay)
            .Distinct()
            .ToListAsync();
        var NhanXetDinhKys = _context.NhanXetDinhKys
            .Where(nx=>ThamGiaLopHocs.Select(tg=>tg.Id).Contains(nx.ThamGiaLopHocId))
            .ProjectTo<NhanXetDinhKyDto>(_mapper.ConfigurationProvider)
            .OrderBy(nx=>nx.STT)
            .ToList();
        return new Output
        {
            isError = false,
            code=200,
            data = new 
            {
                HocSinhCode = ThamGiaLopHocs[0].HocSinh.Code,
                TenHocSinh = ThamGiaLopHocs[0].HocSinh.Ten,
                DenHanNhanXet = true,
                DanhSachNhanXet = NhanXetDinhKys
            },
            message = "Lấy danh sách nhận xét định kỳ thành công"
        };
    }
}

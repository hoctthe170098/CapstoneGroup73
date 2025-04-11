using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;

namespace StudyFlow.Application.BaiKiemTras.Queries.GetLichKiemTraChoGiaoVien;
public record GetLichKiemTraChoGiaoVienQuery : IRequest<Output>
{
    public required string TenLop { get; init; }
}
public class GetLichKiemTraChoGiaoVienQueryHandler : IRequestHandler<GetLichKiemTraChoGiaoVienQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetLichKiemTraChoGiaoVienQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetLichKiemTraChoGiaoVienQuery request, CancellationToken cancellationToken)
    {
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy lịch kiểm tra thành công."
        };
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var UserId = _identityService.GetUserId(token);
        var coSoId = _identityService.GetCampusId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == UserId.ToString());
        if (giaoVien == null) throw new NotFoundIDException();
        var check = await _context.LichHocs
            .AnyAsync(lh=>lh.TrangThai=="Cố định"
            &&lh.GiaoVienCode==giaoVien.Code
            &&lh.TenLop==request.TenLop);
        if(!check) throw new NotFoundIDException();
        var BaiKiemTraList = await  _context.BaiKiemTras
            .Where(kt => kt.LichHoc.TenLop == request.TenLop 
            && kt.LichHoc.GiaoVienCode == giaoVien.Code 
            && kt.LichHoc.TrangThai == "Cố định")
            .OrderByDescending(kt=>kt.NgayKiemTra)
            .ProjectTo<BaiKiemTraDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        output.data = BaiKiemTraList;
        return output;
    }
}

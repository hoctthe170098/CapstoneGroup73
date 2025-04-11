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

namespace StudyFlow.Application.BaiKiemTras.Queries.GetDiemBaiKiemTraChoGiaoVien;
public record GetDiemBaiKiemTraChoGiaoVienQuery : IRequest<Output>
{
    public required string BaiKiemTraId { get; init; }
}
public class GetDiemBaiKiemTraChoGiaoVienQueryHandler : IRequestHandler<GetDiemBaiKiemTraChoGiaoVienQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetDiemBaiKiemTraChoGiaoVienQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetDiemBaiKiemTraChoGiaoVienQuery request, CancellationToken cancellationToken)
    {
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy kết quả bài kiểm tra thành công."
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
        var BaiKiemTra = _context.BaiKiemTras
            .Include(kt=>kt.LichHoc)
            .FirstOrDefault(kt=>kt.Id.ToString()==request.BaiKiemTraId);
        if (BaiKiemTra == null) throw new NotFoundIDException();
        if(BaiKiemTra.LichHoc.GiaoVienCode!=giaoVien.Code) throw new NotFoundIDException();
        var KetQua = await _context.KetQuaBaiKiemTras
            .Where(kq => kq.BaiKiemTraId == BaiKiemTra.Id)
            .ProjectTo<KetQuaBaiKiemTraDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        if(KetQua.Any()) output.data = KetQua;
        else 
        { 
            var HocSinhCode = await _context.ThamGiaLopHocs
                .Where(tg=>tg.LichHocId==BaiKiemTra.LichHocId&&tg.NgayKetThuc>=BaiKiemTra.NgayKiemTra)
                .Select(tg => new
                {
                    tg.HocSinhCode,
                    tg.HocSinh.Ten
                })
                .ToListAsync();
            List<KetQuaBaiKiemTraDto> listKetQua = new List<KetQuaBaiKiemTraDto> ();
            foreach(var item in HocSinhCode)
            {
                var ketQua = new KetQuaBaiKiemTraDto
                {
                    HocSinhCode = item.HocSinhCode,
                    TenHocSinh = item.Ten
                };
                listKetQua .Add(ketQua);
            }
            output.data = listKetQua;
        }
            return output;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiKiemTras.Queries.GetLichKiemTraVaKetQuaChoHocSinh;
public record GetLichKiemTraVaKetQuaChoHocSinhQuery : IRequest<Output>
{
    public required string TenLop { get; init; }
}
public class GetLichKiemTraVaKetQuaChoHocSinhQueryHandler : IRequestHandler<GetLichKiemTraVaKetQuaChoHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetLichKiemTraVaKetQuaChoHocSinhQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetLichKiemTraVaKetQuaChoHocSinhQuery request, CancellationToken cancellationToken)
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
        var HocSinh = await _context.HocSinhs
            .FirstOrDefaultAsync(hs => hs.UserId == UserId.ToString());
        if (HocSinh == null) throw new NotFoundIDException();
        var LichHocCoSinh =  await _context.LichHocs
            .Where(lh=>lh.TrangThai=="Cố định"
            &&lh.TenLop==request.TenLop
            &&lh.ThamGiaLopHocs.Any(tg=>tg.HocSinhCode==HocSinh.Code))
            .Select(lh=>lh.Id)
            .ToListAsync();
        if(!LichHocCoSinh.Any()) throw new NotFoundIDException();
        var BaiKiemTra = await _context.BaiKiemTras
            .Where(kt=>LichHocCoSinh.Contains(kt.LichHocId))
            .OrderByDescending(kt=>kt.NgayKiemTra)
            .ToListAsync();
        var BaiKiemTraList = new List<BaiKiemTraDto>();
        foreach (var kiemTra in BaiKiemTra)
        {
            var baiKiemTra = new BaiKiemTraDto
            {
                NgayKiemTra = (DateOnly)kiemTra.NgayKiemTra!,
                Ten = kiemTra.Ten,
                TrangThai = kiemTra.TrangThai
            };
            var KetQua = _context.KetQuaBaiKiemTras
                .FirstOrDefault(kq=>kq.HocSinhCode==HocSinh.Code&&kq.BaiKiemTraId==kiemTra.Id);
            if (KetQua!=null)
            {
                baiKiemTra.Diem = KetQua.Diem;
                baiKiemTra.NhanXet = KetQua.NhanXet;
            }
            BaiKiemTraList.Add(baiKiemTra);
        }
        output.data = BaiKiemTraList;
        return output;
    }
}

using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemHangNgayCuaHocSinh;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemHangNgayCuaHocSinh;
public record GetBaoCaoDiemHangNgayCuaHocSinhQuery : IRequest<Output>
{
    public required string TenLop { get; set; }
}
public class GetBaoCaoDiemHangNgayCuaHocSinhQueryHandler : IRequestHandler<GetBaoCaoDiemHangNgayCuaHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaoCaoDiemHangNgayCuaHocSinhQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetBaoCaoDiemHangNgayCuaHocSinhQuery request, CancellationToken cancellationToken)
    {
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy báo cáo điểm thành công"
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
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var LichHocCoDinh = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            &&lh.GiaoVienCode==giaoVien.Code
            &&lh.TrangThai=="Cố định").ToList();
        if(!LichHocCoDinh.Any()) throw new NotFoundIDException();
        var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
        var NgayDaHoc = getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau,ngayHienTai);
        var ListHocSinh = _context.ThamGiaLopHocs
            .Where(tg => LichHocCoDinh.Select(lh=>lh.Id).ToList().Contains(tg.LichHocId))
            .Select(tg => new
            {
                tg.HocSinhCode,
                tg.HocSinh.Ten
            })
            .Distinct().ToList();
        var listHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop 
            && lh.GiaoVienCode == giaoVien.Code 
            && lh.TrangThai == "Học bù")
            .ToList();
        foreach(var hocBu in listHocBu)
        {
            if (hocBu.NgayHocGoc < ngayHienTai) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
            if (hocBu.NgayBatDau < ngayHienTai) NgayDaHoc.Add(hocBu.NgayBatDau);
        }
        var data = new List<DiemDanhDto>();
        foreach(var ngay in NgayDaHoc)
        {
            List<DiemDTO> diemDanhs = new List<DiemDTO>();
            var BaoCao = new DiemDanhDto
            {
                Ngay = ngay
            };
            foreach(var hs in ListHocSinh)
            {
                var item = new DiemDTO
                {
                    HocSinhCode = hs.HocSinhCode,
                    TenHocSinh = hs.Ten
                };
                var diemDanh = await _context.DiemDanhs
                    .FirstOrDefaultAsync(dd => dd.Ngay == ngay 
                    && dd.ThamGiaLopHoc.HocSinhCode ==  hs.HocSinhCode
                    && dd.ThamGiaLopHoc.LichHoc.TenLop == request.TenLop);
                if (diemDanh != null)
                {
                    item.DiemBTVN = diemDanh.DiemBTVN;
                    item.DiemTrenLop = diemDanh.DiemTrenLop;
                }
                diemDanhs.Add(item);
            }
            BaoCao.DiemDanhs = diemDanhs.ToList();
            data.Add(BaoCao);
        }
        output.data = data;
        return output;
    }
    private List<DateOnly> getNgayDaHoc(List<int>Thus, DateOnly ngayBatDau, 
        DateOnly ngayHienTai)
    {
        List<DateOnly> ngayDaHocList = new List<DateOnly>();

        for (DateOnly ngay = ngayBatDau; ngay <= ngayHienTai; ngay = ngay.AddDays(1))
        {
            int thu = (int)ngay.DayOfWeek == 0 ? 8 : (int)ngay.DayOfWeek + 1;
            if (Thus.Contains(thu))
            {
                ngayDaHocList.Add(ngay);
            }
        }
        return ngayDaHocList;
    }
        
}

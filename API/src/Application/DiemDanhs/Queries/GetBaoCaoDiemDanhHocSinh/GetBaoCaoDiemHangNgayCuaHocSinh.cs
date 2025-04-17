using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemDanhHocSinh;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemDanhHocSinh;
public record GetBaoCaoDiemDanhHocSinhQuery : IRequest<Output>
{
    public required string TenLop { get; set; }
}
public class GetBaoCaoDiemDanhHocSinhQueryHandler : IRequestHandler<GetBaoCaoDiemDanhHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaoCaoDiemDanhHocSinhQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetBaoCaoDiemDanhHocSinhQuery request, CancellationToken cancellationToken)
    {
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy báo cáo điểm danh thành công"
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
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var LichHocCoDinh = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            &&lh.ThamGiaLopHocs.Select(tg=>tg.HocSinhCode).Contains(HocSinh.Code)
            &&lh.TrangThai=="Cố định")
            .Include(lh=>lh.Phong)
            .Include(lh=>lh.GiaoVien)
            .ToList();
        if(!LichHocCoDinh.Any()) throw new NotFoundIDException();
        var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
        var NgayDaHoc = getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau,ngayHienTai);
        var LichHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop 
            && lh.ThamGiaLopHocs.Select(tg => tg.HocSinhCode).Contains(HocSinh.Code) 
            && lh.TrangThai == "Dạy bù")
            .Include(lh => lh.Phong)
            .Include(lh => lh.GiaoVien)
            .ToList();
        foreach(var hocBu in LichHocBu)
        {
            if (hocBu.NgayHocGoc < ngayHienTai) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
            if (hocBu.NgayBatDau < ngayHienTai) NgayDaHoc.Add(hocBu.NgayBatDau);
        }
        var data = new List<DiemDanhDto>();
        foreach(var ngay in NgayDaHoc)
        {       
            var thu = getThu(ngay);
            var LichHoc = LichHocBu
                .FirstOrDefault(lh=>lh.NgayBatDau == ngay);
            if (LichHoc == null)
            {
                LichHoc = LichHocCoDinh.First(lh=>lh.Thu == thu);
            }
            var lichDayThay = _context.LichHocs
                .Include(lh => lh.GiaoVien)
                .FirstOrDefault(lh=>lh.LichHocGocId==LichHoc.Id&&lh.NgayBatDau==ngay);
            var diemDanhDTO = new DiemDanhDto
            {
                Ngay = ngay,
                TenPhong = LichHoc.Phong.Ten,
                ThoiGianBatDau = LichHoc.GioBatDau,
                ThoiGianKetThuc = LichHoc.GioKetThuc,
                TenGiaoVien = "",
                TinhTrangDiemDanh = ""
            };
            if (lichDayThay != null) diemDanhDTO.TenGiaoVien = lichDayThay.GiaoVien.Ten;
            else diemDanhDTO.TenGiaoVien = LichHoc.GiaoVien.Ten;
            var diemDanh = await _context.DiemDanhs
                    .FirstOrDefaultAsync(dd => dd.Ngay == ngay
                    && dd.ThamGiaLopHoc.HocSinhCode == HocSinh.Code
                    && dd.ThamGiaLopHoc.LichHoc.TenLop == request.TenLop);
            if (diemDanh != null)
            {
                diemDanhDTO.TinhTrangDiemDanh = diemDanh.TrangThai;
            }
            else
            {
                diemDanhDTO.TinhTrangDiemDanh = "Không điểm danh";
            }
            data.Add(diemDanhDTO);
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
    private int getThu(DateOnly ngay)
    {
        var thu = (int)(ngay.DayOfWeek);
        if (thu == 0) return 8; else return thu+1;
    }
}

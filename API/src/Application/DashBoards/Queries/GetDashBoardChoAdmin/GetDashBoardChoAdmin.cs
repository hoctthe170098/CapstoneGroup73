using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.DashBoards.Queries.GetDashBoardChoAdmin;
public record GetDashBoardChoAdminQuery : IRequest<Output>;
public class GetDashBoardChoAdminQueryHandler : IRequestHandler<GetDashBoardChoAdminQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetDashBoardChoAdminQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetDashBoardChoAdminQuery request, CancellationToken cancellationToken)
    {
        var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        int TongSoHocSinh = 0;
        int TongSoGiaoVien = 0;
        int TongSoNhanVien = 0;
        int TongSoLopHoc = 0;
        int TongSoLopHocDangDienRa = 0;
        int TongSoBuoiHoc = 0;
        int TongSoBuoiNghi = 0;
        var listCoSos = await _context.CoSos.Where(c=>c.TrangThai=="open").ToListAsync();
        var ChiaTheoCoSo = new List<CoSoDto>();
        foreach(var coso in  listCoSos)
        {
            var CoSo = new CoSoDto
            {
                TenCoSo = coso.Ten,
                SoGiaoVien = 0,
                SoHocSinh = 0,
                SoLopHoc = 0
            };
            var listHocSinh = await _context.HocSinhs.Where(hs => hs.CoSoId == coso.Id).ToListAsync();
            CoSo.SoHocSinh = listHocSinh.Count();
            TongSoHocSinh += CoSo.SoHocSinh;
            CoSo.SoGiaoVien += _context.GiaoViens.Where(gv=>gv.CoSoId==coso.Id).Count();
            TongSoGiaoVien += CoSo.SoGiaoVien;
            TongSoNhanVien += _context.NhanViens.Where(nv => nv.CoSoId == coso.Id).Count();
            CoSo.SoLopHoc = await _context.LichHocs
                .Where(lh=>lh.TrangThai=="Cố định"&&lh.Phong.CoSoId==coso.Id)
                .Select(lh=>lh.TenLop).Distinct().CountAsync();
            TongSoLopHoc += CoSo.SoLopHoc;
            var soLopHocDangDienRa = await _context.LichHocs
                .Where(lh => lh.TrangThai == "Cố định" 
                && lh.NgayKetThuc >= ngayHienTai 
                && lh.Phong.CoSoId == coso.Id)
                .Select(lh => lh.TenLop)
                .Distinct()
                .CountAsync();
            TongSoLopHocDangDienRa += soLopHocDangDienRa;
            foreach(var HocSinh in listHocSinh)
            {
                GetTiLeDiemDanh(ref TongSoBuoiHoc,ref TongSoBuoiNghi, HocSinh);
            }
            ChiaTheoCoSo.Add(CoSo);
        }
        var listChinhSach = _context.ChinhSaches.ToList();
        var ChiaTheoChinhSach = new List<ChinhSachDto>();
        foreach(var chinhSach in listChinhSach)
        {
            var cs = new ChinhSachDto
            {
                TenChinhSach = chinhSach.Ten,
                SoHocSinh = await _context.HocSinhs
                .Where(hs=>hs.ChinhSachId==chinhSach.Id).CountAsync(),
            };
            ChiaTheoChinhSach.Add(cs);
        }
        return new Output
        {
            isError = false,
            code = 200,
            data = new DashBoardAdminDto
            {
                SoHocSinh = TongSoHocSinh,
                SoGiaoVien = TongSoGiaoVien,
                SoNhanVien = TongSoNhanVien,
                SoLopHoc = TongSoLopHoc,
                SoLopHocDangDiemRa = TongSoLopHocDangDienRa,
                TongSoBuoiHoc = TongSoBuoiHoc,
                TongSoBuoiNghi = TongSoBuoiNghi,
                HocSinhGiaoVienLopHocTheoCoSos = ChiaTheoCoSo.ToList(),
                HocSinhTheoChinhSachs = ChiaTheoChinhSach.ToList()
            },
            message = "Lấy Dashboard thành công."
        };
    }

    private void GetTiLeDiemDanh(ref int tongSoBuoiHoc,ref int tongSoBuoiNghi, HocSinh HocSinh)
    {
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var LopDaHoc = _context.LichHocs
            .Where(lh =>lh.ThamGiaLopHocs.Any(tg=>tg.HocSinhCode==HocSinh.Code))
            .Select(lh=>lh.TenLop)
            .Distinct().ToList();
        foreach(var TenLop in LopDaHoc)
        {
            var LichHocCoDinh = _context.LichHocs
                        .Where(lh => lh.TenLop == TenLop
                        && lh.ThamGiaLopHocs.Select(tg => tg.HocSinhCode).Contains(HocSinh.Code)
                        && lh.TrangThai == "Cố định")
                        .ToList();
            var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
            var NgayDaHoc = (ngayHienTai <= LichHocCoDinh[0].NgayKetThuc)
            ? getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau, ngayHienTai)
            : getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau, LichHocCoDinh[0].NgayKetThuc);
            var LichHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == TenLop
            && lh.ThamGiaLopHocs.Select(tg => tg.HocSinhCode).Contains(HocSinh.Code)
            && lh.TrangThai == "Dạy bù")
            .Include(lh => lh.Phong)
            .Include(lh => lh.GiaoVien)
            .ToList();
            foreach (var hocBu in LichHocBu)
            {
                if (hocBu.NgayHocGoc < ngayHienTai) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
                if (hocBu.NgayBatDau < ngayHienTai) NgayDaHoc.Add(hocBu.NgayBatDau);
            }
            foreach (var ngay in NgayDaHoc)
            {
                var diemDanh =  _context.DiemDanhs
                        .FirstOrDefault(dd => dd.Ngay == ngay
                        && dd.ThamGiaLopHoc.HocSinhCode == HocSinh.Code
                        && dd.ThamGiaLopHoc.LichHoc.TenLop == TenLop);
                if (diemDanh != null)
                {
                    tongSoBuoiHoc++;
                    if (diemDanh.TrangThai == "Vắng mặt") tongSoBuoiNghi++;
                }
            }
        }
    }
    private List<DateOnly> getNgayDaHoc(List<int> Thus, DateOnly ngayBatDau,
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

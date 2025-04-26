using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DashBoards.Queries.GetDashBoardChoAdmin;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.DashBoards.Queries.GetDashBoardChoCampusManager;
public record GetDashBoardChoCampusManagerQuery : IRequest<Output>;
public class GetDashBoardChoCampusManagerQueryHandler : IRequestHandler<GetDashBoardChoCampusManagerQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    public GetDashBoardChoCampusManagerQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetDashBoardChoCampusManagerQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var NgayHomNay = DateOnly.FromDateTime(DateTime.Now);
        var coSoId = _identityService.GetCampusId(token);
        var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        int TongSoHocSinh = 0;
        int TongSoGiaoVien = 0;
        int TongSolopHoc = 0;
        int TiLeDiemDanh = 0;
        var SoLopHocTrong6Thang = new List<SoLopHocTrongThangDto>();
        var diemDanhTheoLops = new List<DiemDanhTheoLopDto>();
        TongSoHocSinh = await _context.HocSinhs.Where(hs => hs.CoSoId == coSoId).CountAsync();
        TongSoGiaoVien = await _context.GiaoViens.Where(gv => gv.CoSoId == coSoId).CountAsync();
        var listChinhSach = _context.ChinhSaches.ToList();
        var ChiaTheoChinhSach = new List<ChinhSachDto>();
        var ThoiGianSuDungPhongs = new List<PhongHocDto>();
        foreach (var chinhSach in listChinhSach)
        {
            var cs = new ChinhSachDto
            {
                TenChinhSach = chinhSach.Ten,
                SoHocSinh = await _context.HocSinhs
                .Where(hs => hs.ChinhSachId == chinhSach.Id && hs.CoSoId == coSoId).CountAsync(),
            };
            ChiaTheoChinhSach.Add(cs);
        }
        var listLopHoc = await _context.LichHocs
            .Where(lh => lh.TrangThai == "Cố định" && lh.Phong.CoSoId == coSoId).ToListAsync();
        TongSolopHoc = listLopHoc.DistinctBy(lh => lh.TenLop).Count();
        var SauThangGanNhat = Get6ThangGanNhat();
        foreach (var BatDauthang in SauThangGanNhat)
        {
            var KetThucThang = BatDauthang.AddMonths(1).AddDays(-1);
            var soLopHocTrongThang = new SoLopHocTrongThangDto
            {
                Thang = BatDauthang.Month,
                Nam = BatDauthang.Year,
                SoLopHoc = 0
            };
            soLopHocTrongThang.SoLopHoc = listLopHoc
                .Where(lh => lh.NgayBatDau <= KetThucThang && lh.NgayKetThuc >= BatDauthang)
                .DistinctBy(lh => lh.TenLop)
                .Count();
            SoLopHocTrong6Thang.Add(soLopHocTrongThang);
        }
        foreach (var tenLop in listLopHoc.Select(lh => lh.TenLop).Distinct())
        {
            var diemDanhTheoLop = new DiemDanhTheoLopDto
            {
                TenLop = tenLop,
                SoBuoiHoc = 0,
                TiLeDiemDanh = 0,
                HocSinhNghiNhieuNhats = new List<HocSinhNghiNhieuNhatDto>()
            };
            diemDanhTheoLop.SoBuoiHoc = await _context.DiemDanhs
                .Where(dd => dd.ThamGiaLopHoc.LichHoc.TenLop == tenLop && dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId)
                .Select(dd => dd.Ngay)
                .Distinct()
                .CountAsync();
            if (diemDanhTheoLop.SoBuoiHoc > 0)
            {
                var DiemDanhCoMat = _context.DiemDanhs
                .Where(dd => dd.ThamGiaLopHoc.LichHoc.TenLop == tenLop
                && dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId
                && dd.TrangThai == "Có mặt")
                .Count();
                var TatCaDiemDanh = _context.DiemDanhs
                    .Where(dd => dd.ThamGiaLopHoc.LichHoc.TenLop == tenLop
                    && dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId)
                    .Count();
                diemDanhTheoLop.TiLeDiemDanh = DiemDanhCoMat * 100 / TatCaDiemDanh;
            }

            diemDanhTheoLop.HocSinhNghiNhieuNhats = _context.DiemDanhs
            .Where(dd => dd.ThamGiaLopHoc.LichHoc.TenLop == tenLop && dd.TrangThai == "Vắng" && dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId)
            .GroupBy(dd => dd.ThamGiaLopHoc.HocSinh)
            .Select(g => new HocSinhNghiNhieuNhatDto
            {
                TenHocSinh = g.Key.Ten,
                SoBuoiNghi = g.Count(),
                HocSinhCode = g.Key.Code,
            })
            .Where(x => x.SoBuoiNghi > 0)
            .OrderByDescending(x => x.SoBuoiNghi)
            .Take(2)
            .ToList();
            diemDanhTheoLops.Add(diemDanhTheoLop);
        }
        var SoDiemDanhNghi = await _context.DiemDanhs
            .Where(dd => dd.TrangThai == "Vắng" && dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId).CountAsync();
        var TongSoDiemDanh = await _context.DiemDanhs
            .Where(dd => dd.ThamGiaLopHoc.LichHoc.Phong.CoSoId == coSoId).CountAsync();
        TiLeDiemDanh = SoDiemDanhNghi * 100 / TongSoDiemDanh;
        var Phong = _context.Phongs.Where(p => p.CoSoId == coSoId).ToList();
        foreach (var p in Phong)
        {
            var thoiGianSuDungPhong = new PhongHocDto { TenPhong = p.Ten, PhanTramThoiGianSuDungPhong = 0 };
            thoiGianSuDungPhong.PhanTramThoiGianSuDungPhong = (int)_context.LichHocs
    .Where(lh => lh.NgayBatDau <= ngayHienTai && lh.NgayKetThuc >= ngayHienTai&&lh.PhongId==p.Id)
    .ToList()
    .Sum(lh => (lh.GioKetThuc - lh.GioBatDau).TotalMinutes) * 100 / (14*60*7);
            ThoiGianSuDungPhongs.Add(thoiGianSuDungPhong);
        }
        return new Output
        {
            isError = false,
            code = 200,
            data = new DashBoardCampusDto
            {
                DiemDanhTheoLops = diemDanhTheoLops.ToList(),
                HocSinhTheoChinhSachs = ChiaTheoChinhSach.ToList(),
                SoLopHoc6ThangGanNhat = SoLopHocTrong6Thang.ToList(),
                ThoiGianSuDungPhongHocs = ThoiGianSuDungPhongs.ToList(),
                SoGiaoVien = TongSoGiaoVien,
                SoHocSinh = TongSoHocSinh,
                SoLopHoc = TongSolopHoc,
                TiLeDiemDanh = TiLeDiemDanh
            },
            message = "Lấy dashboard quản lý cơ sơ thành công."
        };
    }
    private List<DateOnly> Get6ThangGanNhat()
    {
        List<DateOnly> last6Months = new List<DateOnly>();
        DateTime currentDate = DateTime.Now;

        for (int i = 0; i < 6; i++)
        {
            last6Months.Add(DateOnly.FromDateTime(new DateTime(currentDate.Year, currentDate.Month, 1)));
            currentDate = currentDate.AddMonths(-1);
        }

        return last6Months.OrderByDescending(d => d).ToList();
    }
}

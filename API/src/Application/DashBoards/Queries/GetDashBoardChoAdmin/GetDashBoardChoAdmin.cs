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
        int TiLeDiemDanh = 0;
        int TongSoLopHocDangDienRa = 0;
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
            ChiaTheoCoSo.Add(CoSo);
        }
        var TongSoDiemDanhNghi = _context.DiemDanhs.Where(dd => dd.TrangThai == "Vắng").Count();
        var TongSoDiemDanh = _context.DiemDanhs.Count();
        TiLeDiemDanh = TongSoDiemDanhNghi * 100 / TongSoDiemDanh;
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
                TiLeDiemDanh = TiLeDiemDanh,
                SoLopHocDangDiemRa = TongSoLopHocDangDienRa,
                HocSinhGiaoVienLopHocTheoCoSos = ChiaTheoCoSo.ToList(),
                HocSinhTheoChinhSachs = ChiaTheoChinhSach.ToList()
            },
            message = "Lấy Dashboard thành công."
        };
    }
}

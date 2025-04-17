using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemHocSinh;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoTatCaDiemChoHocSinh;

public class GetBaoCaoDiemHocSinhQuery : IRequest<Output>
{
    public required string TenLop { get; set; }
}

public class GetBaoCaoDiemHocSinhQueryHandler : IRequestHandler<GetBaoCaoDiemHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaoCaoDiemHocSinhQueryHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetBaoCaoDiemHocSinhQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token);
        if (userId == Guid.Empty) throw new NotFoundIDException();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(hs => hs.UserId == userId.ToString(), cancellationToken)
            ?? throw new NotFoundIDException();

        var thamGiaIds = await _context.ThamGiaLopHocs
            .Where(tg => tg.HocSinhCode == hocSinh.Code && tg.LichHoc.TenLop == request.TenLop)
            .Select(tg => tg.Id)
            .ToListAsync(cancellationToken);

        var diemDanhList = await _context.DiemDanhs
            .Where(dd => thamGiaIds.Contains(dd.ThamGiaLopHocId))
            .ToListAsync(cancellationToken);

        var baiKiemTraList = await _context.KetQuaBaiKiemTras
            .Include(kq => kq.BaiKiemTra)
            .Where(kq => kq.HocSinhCode == hocSinh.Code && kq.BaiKiemTra.LichHoc.TenLop == request.TenLop)
            .ToListAsync(cancellationToken);

        var nhanXetDinhKy = diemDanhList
            .Where(dd => !string.IsNullOrWhiteSpace(dd.NhanXet))
            .OrderByDescending(dd => dd.Ngay)
            .Select(dd => new NhanXetDto
            {
                Ngay = dd.Ngay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                NhanXet = dd.NhanXet
            }).ToList();

        var diemTrenLopChiTiet = diemDanhList
            .Where(dd => dd.DiemTrenLop.HasValue)
            .Select(dd => new DiemChiTietDto
            {
                Ngay = dd.Ngay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Diem = dd.DiemTrenLop?.ToString() ?? "N/A",
                NhanXet = dd.NhanXet
            }).ToList();

        var diemBaiTapVeNha = diemDanhList
            .Where(dd => dd.DiemBTVN.HasValue)
            .Select(dd => new DiemChiTietDto
            {
                Ngay = dd.Ngay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Diem = dd.DiemBTVN?.ToString() ?? "N/A",
                NhanXet = dd.NhanXet
            }).ToList();

        var diemKiemTra = baiKiemTraList
            .Select(kq => new DiemKiemTraDto
            {
                Ten = kq.BaiKiemTra.Ten,
                NgayKiemTra = kq.BaiKiemTra.NgayKiemTra?.ToString("dd/MM/yyyy") ?? "",
                TrangThai = kq.BaiKiemTra.TrangThai,
                Diem = kq.Diem?.ToString("0.##") ?? "",
                NhanXet = kq.NhanXet
            }).ToList();

        double diemTrenLopTB = diemTrenLopChiTiet.Any()
            ? diemTrenLopChiTiet.Average(x => double.Parse(x.Diem))
            : 0;

        double diemBTVNTB = diemBaiTapVeNha.Any()
            ? diemBaiTapVeNha.Average(x => double.Parse(x.Diem))
            : 0;
        double DiemKiemTraTB = diemKiemTra.Any()
            ? diemKiemTra.Average(x => double.Parse(x.Diem))
            : 0;
        var result = new BaoCaoHocSinhDto
        {
            Ten = hocSinh.Ten,
            Code = hocSinh.Code,
            DiemTrenLopTB = Math.Round(diemTrenLopTB, 2),
            DiemBaiTapTB = Math.Round(diemBTVNTB, 2),
            DiemKiemTraTB = Math.Round(DiemKiemTraTB,2),
            NhanXetDinhKy = nhanXetDinhKy,
            DiemTrenLopChiTiet = diemTrenLopChiTiet,
            DiemBaiTapVeNha = diemBaiTapVeNha,
            DiemKiemTra = diemKiemTra
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = result,
            message = "Lấy báo cáo học sinh thành công"
        };
    }
}

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

        var nhanXetDinhKyList = await _context.NhanXetDinhKys
            .Where(nx => thamGiaIds.Contains(nx.ThamGiaLopHocId))
            .OrderByDescending(nx => nx.NgayNhanXet)
            .Select(nx => new NhanXetDto
            {
                Ngay = nx.NgayNhanXet.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                NhanXet = nx.NoiDungNhanXet
            })
            .ToListAsync(cancellationToken);

        var diemHangNgay = diemDanhList
            .OrderByDescending(dd => dd.Ngay)
            .Select(dd => new DiemHangNgayDto
            {
                Ngay = dd.Ngay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                DiemTrenLop = dd.DiemTrenLop.HasValue ? $"{dd.DiemTrenLop:0.##}/10" : "N/A",
                DiemBTVN = dd.DiemBTVN.HasValue ? $"{dd.DiemBTVN:0.##}/10" : "N/A",
                NhanXet = dd.NhanXet
            })
            .ToList();

        double diemTrenLopTB = diemDanhList.Any(dd => dd.DiemTrenLop.HasValue)
            ? diemDanhList.Where(dd => dd.DiemTrenLop.HasValue).Average(dd => dd.DiemTrenLop!.Value)
            : 0;

        double diemBTVNTB = diemDanhList.Any(dd => dd.DiemBTVN.HasValue)
            ? diemDanhList.Where(dd => dd.DiemBTVN.HasValue).Average(dd => dd.DiemBTVN!.Value)
            : 0;

        double diemKiemTraTB = baiKiemTraList.Any(kq => kq.Diem.HasValue)
            ? baiKiemTraList.Where(kq => kq.Diem.HasValue).Average(kq => kq.Diem!.Value)
            : 0;

        var result = new BaoCaoHocSinhDto
        {
            Ten = hocSinh.Ten,
            Code = hocSinh.Code,
            DiemTrenLopTB = Math.Round(diemTrenLopTB, 2),
            DiemBaiTapTB = Math.Round(diemBTVNTB, 2),
            DiemKiemTraTB = Math.Round(diemKiemTraTB, 2),
            NhanXetDinhKy = nhanXetDinhKyList,
            DiemHangNgay = diemHangNgay
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

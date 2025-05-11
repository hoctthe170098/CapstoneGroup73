using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemDanhChoTungLop;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemDanhChoTungLop;
public record GetBaoCaoDiemDanhChoTungLopQuery : IRequest<Output>
{
    public required string TenLop { get; set; }
    public DateOnly? Ngay { get; set; }
}
public class GetBaoCaoDiemDanhChoTungLopQueryHandler : IRequestHandler<GetBaoCaoDiemDanhChoTungLopQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaoCaoDiemDanhChoTungLopQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetBaoCaoDiemDanhChoTungLopQuery request, CancellationToken cancellationToken)
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
        var coSoId = _identityService.GetCampusId(token);
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var LichHocCoDinh = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            && lh.Phong.CoSoId == coSoId
            && lh.TrangThai == "Cố định").ToList();
        if (!LichHocCoDinh.Any()) throw new NotFoundIDException();
        var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
        var NgayDaHoc = (ngayHienTai <= LichHocCoDinh[0].NgayKetThuc) 
            ? getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau, ngayHienTai) 
            : getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau, LichHocCoDinh[0].NgayKetThuc);
        var listHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            && lh.Phong.CoSoId == coSoId
            && lh.TrangThai == "Dạy bù")
            .ToList();
        foreach (var hocBu in listHocBu)
        {
            if (hocBu.NgayHocGoc < ngayHienTai) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
            if (hocBu.NgayBatDau < ngayHienTai && hocBu.NgayBatDau!=DateOnly.MinValue) NgayDaHoc.Add(hocBu.NgayBatDau);
        }
        NgayDaHoc = NgayDaHoc.OrderByDescending(ng => ng).ToList();
        if (NgayDaHoc.Count() == 0) throw new Exception("Lớp chưa học ngày nào, chưa thể hiển thị báo cáo.");
        DateOnly NgayCanLay = DateOnly.MinValue;
        if (request.Ngay == null) NgayCanLay = NgayDaHoc[0];
        else
        {
            if (!NgayDaHoc.Contains((DateOnly)request.Ngay)) throw new NotFoundIDException();
            else NgayCanLay = (DateOnly)request.Ngay;
        }
        var ListHocSinh = _context.ThamGiaLopHocs
    .Where(tg => LichHocCoDinh.Select(lh => lh.Id).ToList().Contains(tg.LichHocId)&&tg.NgayKetThuc>=NgayCanLay&&tg.NgayBatDau<=NgayCanLay)
    .Select(tg => new
    {
        tg.HocSinhCode,
        tg.HocSinh.Ten
    })
    .Distinct().ToList();
        var data = new List<BaoCaoHocPhiDto>();
        List<DiemDanhDto> diemDanhs = new List<DiemDanhDto>();
        var BaoCao = new BaoCaoHocPhiDto
        {
            Ngay = NgayCanLay
        };
        foreach (var hs in ListHocSinh)
        {
            var item = new DiemDanhDto
            {
                HocSinhCode = hs.HocSinhCode,
                TenHocSinh = hs.Ten
            };
            var diemDanh = await _context.DiemDanhs
                .FirstOrDefaultAsync(dd => dd.Ngay == NgayCanLay
                && dd.ThamGiaLopHoc.HocSinhCode == hs.HocSinhCode
                && dd.ThamGiaLopHoc.LichHoc.TenLop == request.TenLop);
            if (diemDanh != null)
            {
                item.Id = diemDanh.Id;
                item.TrangThai = diemDanh.TrangThai;
            }
            else item.TrangThai = "Không điểm danh";
            diemDanhs.Add(item);
        }
        BaoCao.DiemDanhs = diemDanhs.ToList();
        BaoCao.Ngays = NgayDaHoc;
        output.data = BaoCao;
        return output;
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

using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoHocPhiChoTungLop;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Queries.GetBaoCaoHocPhiChoTungLop;
public record GetBaoCaoHocPhiChoTungLopQuery : IRequest<Output>
{
    public required string TenLop { get; set; }
    public int? Thang { get; set; }
    public int? Nam { get; set; }
}
public class GetBaoCaoHocPhiChoTungLopQueryHandler : IRequestHandler<GetBaoCaoHocPhiChoTungLopQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetBaoCaoHocPhiChoTungLopQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetBaoCaoHocPhiChoTungLopQuery request, CancellationToken cancellationToken)
    {
        if ((request.Thang == null && request.Nam != null) 
            || (request.Thang != null && request.Nam == null)) throw new FormatException(); 
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy báo cáo học phí thành công"
        };
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var LichHocCoDinh = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            &&lh.Phong.CoSoId == coSoId
            &&lh.TrangThai=="Cố định").ToList();
        if(!LichHocCoDinh.Any()) throw new NotFoundIDException();
        var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
        var NgayDaHoc = getNgayDaHoc(Thus, LichHocCoDinh[0].NgayBatDau,ngayHienTai);
        var HocPhi1Buoi = LichHocCoDinh[0].HocPhi;
        var ListHocSinh = _context.ThamGiaLopHocs
            .Where(tg => LichHocCoDinh.Select(lh=>lh.Id).ToList().Contains(tg.LichHocId))
            .Select(tg => new
            {
                tg.HocSinhCode,
                tg.HocSinh.Ten,
                tg.HocSinh.ChinhSachId,
            })
            .Distinct().ToList();
        var listHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop 
            && lh.Phong.CoSoId == coSoId 
            && lh.TrangThai == "Dạy bù")
            .ToList();
        foreach(var hocBu in listHocBu)
        {
            if (hocBu.NgayHocGoc < ngayHienTai) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
            if (hocBu.NgayBatDau < ngayHienTai && hocBu.NgayBatDau != DateOnly.MinValue) NgayDaHoc.Add(hocBu.NgayBatDau);
        }
        if (!NgayDaHoc.Any()) throw new Exception("Lớp học này chưa học buổi nào.");
        var monthYearFirstOccurrence = new Dictionary<ThangNamDto, DateOnly>();
        foreach (var day in NgayDaHoc)
        {
            ThangNamDto ThangNam = new ThangNamDto
            {
                Thang = day.Month,
                Nam = day.Year
            };
            if (!monthYearFirstOccurrence.ContainsKey(ThangNam))
            {
                monthYearFirstOccurrence.Add(ThangNam, day); // Lưu ngày đầu tiên gặp của tháng-năm
            }
        }
        // 2. Sắp xếp các cặp tháng-năm dựa trên ngày xuất hiện đầu tiên
        var sortedMonthYears = monthYearFirstOccurrence.OrderBy(pair => pair.Value)
                                                        .Select(pair => pair.Key)
                                                        .ToList();
        var ThangDaHoc = NgayDaHoc.Select(ng => ng.Month).Distinct().OrderBy(th => th).ToList();
        int ThangCanLay = 0;
        int NamCanLay = 0;
        if (request.Thang == null&&request.Nam==null)
        {
            ThangCanLay = sortedMonthYears[0].Thang;
            NamCanLay = sortedMonthYears[0].Nam;
        }
        else
        {
            var ThangNamCanLay = sortedMonthYears
                .FirstOrDefault(t => t.Thang == request.Thang && t.Nam == request.Nam);
            if (ThangNamCanLay == null) throw new NotFoundDataException();
            ThangCanLay = ThangNamCanLay.Thang;
            NamCanLay = ThangNamCanLay.Nam;
        }
        var NgayCanLays = NgayDaHoc.Where(ng=>ng.Month == ThangCanLay&&ng.Year==NamCanLay).ToList();
        var data = new BaoCaoHocPhiDto
        {
            Thang = ThangCanLay,
            Nam = NamCanLay,
            ThangNams = sortedMonthYears.ToList(),
            HocPhis = new List<HocPhiDto>()
        };
        List<HocPhiDto> hocPhis = new List<HocPhiDto>();
        foreach (var hs in ListHocSinh)
        {
            float phanTramGiam = 0;
            var hocPhi = new HocPhiDto
            {
                HocSinhCode = hs.HocSinhCode,
                TenHocSinh = hs.Ten,
                HocPhi1Buoi = HocPhi1Buoi,
            };
            if (hs.ChinhSachId == null) hocPhi.SoPhanTramGiam = 0;
            else
            {
                var chinhSach = _context.ChinhSaches.First(cs => cs.Id == hs.ChinhSachId);
                phanTramGiam = chinhSach.PhanTramGiam;
                hocPhi.SoPhanTramGiam = (int)(phanTramGiam * 100);
            }
            var soBuoiHoc = 0;
            var soBuoiNghi = 0;
            foreach(var ngayHoc in NgayCanLays)
            {
                var diemDanh = await _context.DiemDanhs
                .FirstOrDefaultAsync(dd => dd.Ngay == ngayHoc
                && dd.ThamGiaLopHoc.HocSinhCode == hs.HocSinhCode
                && dd.ThamGiaLopHoc.LichHoc.TenLop == request.TenLop);
                if (diemDanh != null)
                {
                    if(diemDanh.TrangThai=="Vắng") soBuoiNghi++;
                    else soBuoiHoc++;
                }
            }
            hocPhi.SoBuoiHoc = soBuoiHoc;
            hocPhi.SoBuoiNghi = soBuoiNghi;
            hocPhi.TongHocPhi = hocPhi.SoBuoiHoc * HocPhi1Buoi * (1 - phanTramGiam);
            hocPhis.Add(hocPhi);
        }
        data.HocPhis = hocPhis.ToList();
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

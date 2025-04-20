using System.Globalization;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.LichHocs.Queries.GetLichHocHocSinh;
public record GetLichHocHocSinhQuery : IRequest<Output>
{
    public int? Tuan {  get; init; }
    public int? Nam { get; init; }
}
public class GetLichHocHocSinhQueryHandler : IRequestHandler<GetLichHocHocSinhQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetLichHocHocSinhQueryHandler(IApplicationDbContext context
        ,IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService
        , IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetLichHocHocSinhQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var UserId = _identityService.GetUserId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var hocSinh = await _context.HocSinhs
            .FirstOrDefaultAsync(gv => gv.UserId == UserId.ToString());
        if (hocSinh == null) throw new NotFoundIDException();
        var data = new GetLichHocHocSinhByThuVaNamDto();
        var lichHocCaTuan = new List<LichHocTrongNgayDto>();
        List<DateOnly> ngayTrongTuanList = new List<DateOnly>();
        if (request.Tuan == null && request.Nam == null)
        {
            DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
            int tuanThu = ISOWeek.GetWeekOfYear(ngayHienTai.ToDateTime(TimeOnly.MinValue));
            data.Tuan = tuanThu;
            data.Nam = ngayHienTai.Year;
            // Xác định ngày đầu tuần (thứ Hai)
            DayOfWeek ngayTrongTuan = ngayHienTai.DayOfWeek;
            int ngayLech = ngayTrongTuan == DayOfWeek.Sunday ? 6 : (int)ngayTrongTuan - (int)DayOfWeek.Monday;
            DateOnly ngayDauTuan = ngayHienTai.AddDays(-ngayLech);

            // Tạo danh sách 7 ngày trong tuần

            for (int i = 0; i < 7; i++)
            {
                ngayTrongTuanList.Add(ngayDauTuan.AddDays(i));
            }
        }else if(request.Tuan != null && request.Nam != null)
        {
            int nam = request.Nam.Value;
            int tuan = request.Tuan.Value;
            data.Tuan = tuan;
            data.Nam = nam;
            // Xác định ngày đầu tiên của tuần
            DateTime ngayDauTuanDateTime = ISOWeek.ToDateTime(nam, tuan, DayOfWeek.Monday);
            DateOnly ngayDauTuan = DateOnly.FromDateTime(ngayDauTuanDateTime);

            // Tạo danh sách 7 ngày trong tuần
            for (int i = 0; i < 7; i++)
            {
                ngayTrongTuanList.Add(ngayDauTuan.AddDays(i));
            }
        }
        else throw new NotFoundDataException();
            for (int i = 2; i <= ngayTrongTuanList.Count+1; i++)
            {
                var lichHocTrongNgay = new LichHocTrongNgayDto
                {
                    thu = i,
                    Ngay = ngayTrongTuanList[i - 2],
                    Lops = new List<LopHocDto>()
                };
                var lichHoc = await _context.LichHocs
                    .Where(lh => lh.ThamGiaLopHocs.Select(tg=>tg.HocSinhCode).Contains(hocSinh.Code)
                    && lh.Thu == i && lh.NgayBatDau <= ngayTrongTuanList[i - 2]
                    && lh.NgayKetThuc >= ngayTrongTuanList[i - 2])
                    .Include(lh => lh.ChuongTrinh)
                    .Include(lh => lh.Phong)
                    .ToListAsync();
                var listLopHoc = new List<LopHocDto>();
                foreach (var lich in lichHoc)
                {
                    var check = await _context.LichHocs
                        .AnyAsync(lh => lh.LichHocGocId == lich.Id
                        && (lh.TrangThai == "Dạy bù" || lh.TrangThai == "Dạy thay")
                        && lh.NgayHocGoc == ngayTrongTuanList[i - 2]);
                    if (!check)
                    {
                        var lopHoc = new LopHocDto
                        {
                            TenLop = lich.TenLop,
                            TenChuongTrinh = lich.ChuongTrinh.TieuDe,
                            TenPhong = lich.Phong.Ten,
                            GioBatDau = lich.GioBatDau,
                            GioKetThuc = lich.GioKetThuc,
                            TrangThai = lich.TrangThai
                        };
                        listLopHoc.Add(lopHoc);
                    }
                }
                listLopHoc = listLopHoc.OrderBy(l => l.GioBatDau).ToList();
                lichHocTrongNgay.Lops = listLopHoc;
                lichHocCaTuan.Add(lichHocTrongNgay);
            }
        data.LichHocCaTuans = lichHocCaTuan;
        return new Output
        {
            isError = false,
            code = 200,
            data = data,
            message = "Lấy danh sách lịch học thành công"
        };
    }
}

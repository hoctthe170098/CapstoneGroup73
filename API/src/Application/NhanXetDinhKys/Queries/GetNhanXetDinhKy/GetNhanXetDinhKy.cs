using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using StudyFlow.Domain.Enums;

namespace StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
public record GetNhanXetDinhKysQuery : IRequest<Output>
{
    public required string TenLop {  get; set; }
    public required string HocSinhCode {  get; set; }
}
public class GetNhanXetDinhKysQueryHandler : IRequestHandler<GetNhanXetDinhKysQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetNhanXetDinhKysQueryHandler(IApplicationDbContext context, IMapper mapper
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetNhanXetDinhKysQuery request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var ThamGiaLopHoc = await _context.ThamGiaLopHocs
            .Where(tg=>tg.HocSinhCode==request.HocSinhCode
            &&tg.LichHoc.TenLop==request.TenLop
            &&tg.LichHoc.Phong.CoSoId==coSoId
            &&tg.LichHoc.TrangThai=="Cố định")
            .Include(tg=>tg.LichHoc)
            .Include(tg=>tg.HocSinh)
            .ToListAsync();
        var LichHocCoDinh = ThamGiaLopHoc.Select(tg=>tg.LichHoc).ToList();
        if (!LichHocCoDinh.Any()) throw new NotFoundIDException();
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
        var NgayKetThuc = ThamGiaLopHoc[0].NgayKetThuc;
        if(NgayKetThuc>ngayHienTai) NgayKetThuc = ngayHienTai;
        var NgayDaHoc = getNgayDaHoc(Thus, ThamGiaLopHoc[0].NgayBatDau, NgayKetThuc);
        var listHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            && lh.Phong.CoSoId == coSoId
            && lh.TrangThai == "Học bù")
            .ToList();
        foreach (var hocBu in listHocBu)
        {
            if (hocBu.NgayHocGoc < NgayKetThuc) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
            if (hocBu.NgayBatDau < NgayKetThuc) NgayDaHoc.Add(hocBu.NgayBatDau);
        }
        var NhanXetDinhKys = _context.NhanXetDinhKys
            .Where(nx=>ThamGiaLopHoc.Select(tg=>tg.Id).Contains(nx.ThamGiaLopHocId))
            .ProjectTo<NhanXetDinhKyDto>(_mapper.ConfigurationProvider)
            .OrderBy(nx=>nx.STT)
            .ToList();
        return new Output
        {
            isError = false,
            code=200,
            data = new 
            {
                HocSinhCode = ThamGiaLopHoc[0].HocSinh.Code,
                TenHocSinh = ThamGiaLopHoc[0].HocSinh.Ten,
                DenHanNhanXet = (NgayDaHoc.Count/4>NhanXetDinhKys.Count),
                DanhSachNhanXet = NhanXetDinhKys
            },
            message = "Lấy danh sách nhận xét định kỳ thành công"
        };
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

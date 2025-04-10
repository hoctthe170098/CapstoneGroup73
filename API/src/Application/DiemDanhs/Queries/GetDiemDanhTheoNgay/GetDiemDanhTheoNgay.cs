using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Queries.GetDiemDanhTheoNgay;
using StudyFlow.Application.TodoLists.Queries.GetTodos;
using StudyFlow.Domain.Entities;
using StudyFlow.Domain.Enums;

namespace StudyFlow.Application.Cosos.Queries.GetDiemDanhTheoNgay;
public record GetDiemDanhTheoNgayQuery : IRequest<Output>
{
    public required string TenLop { get; set; }
}
public class GetDiemDanhTheoNgayQueryHandler : IRequestHandler<GetDiemDanhTheoNgayQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetDiemDanhTheoNgayQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }
    public async Task<Output> Handle(GetDiemDanhTheoNgayQuery request, CancellationToken cancellationToken)
    {
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy điểm danh thành công"
        };
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var UserId = _identityService.GetUserId(token);
        var coSoId = _identityService.GetCampusId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == UserId.ToString());
        if (giaoVien == null) throw new NotFoundIDException();
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        int thu = (int)ngayHienTai.DayOfWeek;
        if (thu == 0) thu = 8; else thu++;
        var lichHocCoDinh = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            && lh.GiaoVienCode == giaoVien.Code
            && lh.TrangThai == "Cố định").ToList();
        if (lichHocCoDinh.Any())
        {
            var lichHocHomNay = lichHocCoDinh.FirstOrDefault(lh => lh.Thu == thu);
            if (lichHocHomNay == null)
            {
                var lichDayBu = _context.LichHocs
                    .FirstOrDefault(lh => lh.TenLop == request.TenLop
                    && lh.GiaoVienCode == giaoVien.Code
                    && lh.TrangThai == "Dạy bù"
                    && lh.NgayBatDau == ngayHienTai);
                if (lichDayBu == null) throw new Exception("Hôm nay không có lịch dạy, không thể điểm danh");
                else lichHocHomNay = lichDayBu;
            }
            else
            {
                var checklichNghi = _context.LichHocs
                    .Any(lh => lh.LichHocGocId == lichHocHomNay.Id 
                    && lh.NgayHocGoc == ngayHienTai 
                    && lh.TrangThai == "Dạy bù");
                if (checklichNghi) throw new Exception("lớp hôm nay nghỉ, không thể điểm danh");
                var checkLichDayThay = _context.LichHocs
                        .Any(lh => lh.LichHocGocId == lichHocHomNay.Id 
                        && lh.NgayHocGoc == ngayHienTai 
                        && lh.TrangThai == "Dạy thay");
                if (checkLichDayThay)
                    throw new Exception("Lớp hôm nay đã được dạy thay, không thể điểm danh");
            }
            output = getDiemDanh(output, lichHocHomNay, ngayHienTai, cancellationToken);
        }
        else
        {
            var lichDayThay = _context.LichHocs
                .FirstOrDefault(lh => lh.TrangThai == "Dạy thay"
                && lh.TenLop == request.TenLop
                && lh.GiaoVienCode == giaoVien.Code
                && lh.Phong.CoSoId == coSoId);
            if (lichDayThay == null) throw new NotFoundIDException();
            if (lichDayThay.NgayBatDau != DateOnly.FromDateTime(DateTime.Now)) 
                throw new Exception("Hôm nay không phải ngày dạy thay của bạn, không thể điểm danh.");
            var lichHocHomNay = _context.LichHocs.First(lh => lh.Id == lichDayThay!.LichHocGocId);
            output = getDiemDanh(output, lichHocHomNay, ngayHienTai,cancellationToken);
        }
        return output;
    }
    private Output getDiemDanh(Output output,LichHoc lichHocHomNay
        , DateOnly ngayHienTai, CancellationToken cancellationToken)
    {
        if (lichHocHomNay.GioBatDau > TimeOnly.FromDateTime(DateTime.Now))
            throw new Exception("Chưa đến giờ điểm danh, không thể điểm danh");
        var tonTaiDiemDanh = _context.DiemDanhs
            .Any(d => d.Ngay == ngayHienTai && d.ThamGiaLopHoc.LichHocId == lichHocHomNay.Id);
        if (!tonTaiDiemDanh)
        {
            var thamGiaLopHoc = _context.ThamGiaLopHocs
                .Where(tg => tg.LichHocId == lichHocHomNay.Id
                &&tg.NgayKetThuc>= DateOnly.FromDateTime(DateTime.Now))
                .Select(tg => tg.Id)
                .ToList();
            var listDiemDanhTaoMoi = new List<DiemDanh>();
            foreach (var thamGia in thamGiaLopHoc)
            {
                var diemDanh = new DiemDanh
                {
                    Id = Guid.NewGuid(),
                    ThamGiaLopHocId = thamGia,
                    TrangThai = "Vắng",
                    Ngay = ngayHienTai
                };
                listDiemDanhTaoMoi.Add(diemDanh);
            }
            _context.DiemDanhs.AddRange(listDiemDanhTaoMoi);
            _context.SaveChangesAsync(cancellationToken);
        }
        var diemDanhHomNay = _context.DiemDanhs
                        .Where(dd => dd.ThamGiaLopHoc.LichHocId == lichHocHomNay.Id
                        && dd.Ngay == ngayHienTai)
                        .Include(dd => dd.ThamGiaLopHoc)
                        .ThenInclude(dd => dd.HocSinh)
                        .OrderBy(dd => dd.ThamGiaLopHoc.HocSinhCode)
                        .ProjectTo<DiemDanhDto>(_mapper.ConfigurationProvider)
                        .ToList();
        output.data = diemDanhHomNay;
        return output;
    }
}

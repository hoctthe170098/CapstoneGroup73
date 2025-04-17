using System.Drawing.Drawing2D;
using CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ChinhSachs.Commands.DeleteLichHoc;

public record DeleteLichHocCommand(string TenLopHoc) : IRequest<Output>;

public class DeleteLichHocCommandHandler : IRequestHandler<DeleteLichHocCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public DeleteLichHocCommandHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(DeleteLichHocCommand request, CancellationToken cancellationToken)
    {
        var tenlopHoc = request.TenLopHoc.ToLower().Trim();
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichHoc = await _context.LichHocs
            .Where(lh=>lh.TenLop== request.TenLopHoc&&lh.Phong.CoSoId==coSoId).ToListAsync();
        if (!lichHoc.Any()) throw new NotFoundIDException();
        string message="";
        var lichHocCoDinh = lichHoc.First(lh => lh.TrangThai == "Cố định");
        var HomNay = DateOnly.FromDateTime(DateTime.Now);
        var ThuHomNay = (int)HomNay.DayOfWeek > 0 ? (int)(HomNay.DayOfWeek) + 1 : 8;
        var TonTaiLichHomNay = await _context.LichHocs
            .AnyAsync(lh => lh.Phong.CoSoId == coSoId && lh.TenLop == request.TenLopHoc
            && ((lh.Thu == ThuHomNay && lh.TrangThai == "Cố định")
            || (lh.TrangThai == "Dạy bù" && lh.NgayKetThuc == HomNay)));
        if (TonTaiLichHomNay) throw new Exception("Hôm nay lớp có lịch học, vui lòng chờ đến ngày mai để xoá ");
        if (lichHocCoDinh.NgayBatDau > HomNay)
        {
            var thamGiaLopHoc = await _context.ThamGiaLopHocs
                .Where(tg => lichHoc.Select(lh => lh.Id).Contains(tg.LichHocId))
                .ToListAsync();
            _context.ThamGiaLopHocs.RemoveRange(thamGiaLopHoc);
            _context.LichHocs.RemoveRange(lichHoc);
            message = "Xoá lớp học thành công.";
        }
        else 
        {
            var lichDayBuDayThay = lichHoc
                .Where(lh => (lh.TrangThai == "Dạy thay" || lh.TrangThai == "Dạy bù")&&lh.NgayKetThuc>HomNay)
                .ToList();
            var ThamGiaDayBuDayThay = _context.ThamGiaLopHocs
                .Where(tg=>lichDayBuDayThay.Select(lh=>lh.Id).Contains(tg.LichHocId)).ToList();
            _context.ThamGiaLopHocs.RemoveRange(ThamGiaDayBuDayThay);
            _context.LichHocs.RemoveRange(lichDayBuDayThay);
            var lichCoDinh = lichHoc.Where(lh=>lh.TrangThai=="Cố định").ToList();
            var ThamGiaCoDinh = _context.ThamGiaLopHocs
                .Where(tg=>lichCoDinh.Select(lh=>lh.Id).Contains(tg.LichHocId)&&tg.NgayKetThuc>HomNay).ToList();
            foreach(var tg in ThamGiaCoDinh)
            {
                tg.NgayKetThuc = HomNay;
            }
            foreach(var lh in lichCoDinh)
            {
                lh.NgayKetThuc = HomNay;
            }
            message = "Đóng lớp học thành công.";
        }
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = null,
            code = 200,
            message = message
        };
    }
}

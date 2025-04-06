using CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ChinhSachs.Commands.DeleteLichDayBu;

public record DeleteLichDayBuCommand(Guid lichHocId) : IRequest<Output>;

public class DeleteLichDayBuCommandHandler : IRequestHandler<DeleteLichDayBuCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public DeleteLichDayBuCommandHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(DeleteLichDayBuCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichDayBu = await _context.LichHocs
            .FirstOrDefaultAsync(lh => lh.Id == request.lichHocId 
            && lh.TrangThai == "Dạy bù"
            && lh.Phong.CoSoId == coSoId
            && lh.NgayKetThuc != DateOnly.MinValue);
        if (lichDayBu == null) throw new Exception("Lịch dạy bù không tồn tại");
        if (lichDayBu.NgayKetThuc <= DateOnly.FromDateTime(DateTime.Now))
            throw new Exception("Lịch dạy bù đã quá hạn xoá.");
        var thamGiaLopHoc = _context.ThamGiaLopHocs.Where(tg => tg.LichHocId == lichDayBu.Id).ToList();
        _context.ThamGiaLopHocs.RemoveRange(thamGiaLopHoc);
        lichDayBu.NgayBatDau = DateOnly.MinValue;
        lichDayBu.NgayKetThuc = DateOnly.MinValue;
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = null,
            code = 200,
            message = "Xóa lịch dạy bù thành công."
        };
    }
}

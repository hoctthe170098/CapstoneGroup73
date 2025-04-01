using CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ChinhSachs.Commands.DeleteLichDayThay;

public record DeleteLichDayThayCommand(Guid lichHocId) : IRequest<Output>;

public class DeleteLichDayThayCommandHandler : IRequestHandler<DeleteLichDayThayCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public DeleteLichDayThayCommandHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(DeleteLichDayThayCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichDayThay = await _context.LichHocs
            .FirstOrDefaultAsync(lh => lh.Id == request.lichHocId 
            && lh.TrangThai == "Dạy thay"
            && lh.Phong.CoSoId == coSoId 
            && lh.NgayKetThuc > DateOnly.FromDateTime(DateTime.Now));
        if (lichDayThay == null) throw new Exception("Lịch học đã bắt đầu, không thể xoá.");
        _context.LichHocs.Remove(lichDayThay);
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = null,
            code = 200,
            message = "Xóa lịch dạy thêm thành công."
        };
    }
}

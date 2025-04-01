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
            .Where(b=>b.TenLop.ToLower() == tenlopHoc
            && b.Phong.CoSoId==coSoId).ToListAsync();
        if (!lichHoc.Any()) throw new NotFoundIDException();
        foreach(var lh in lichHoc)
        {
            if (lh.NgayBatDau <= DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Lớp học này đã bắt đầu, không thể xoá");
        }
        var thamGiaLopHoc = await _context.ThamGiaLopHocs
            .Where(tg=>lichHoc.Select(lh=>lh.Id).ToList().Contains(tg.LichHocId)).ToListAsync();
        _context.ThamGiaLopHocs.RemoveRange(thamGiaLopHoc);
        await _context.SaveChangesAsync(cancellationToken);
        _context.LichHocs.RemoveRange(lichHoc);
        await _context.SaveChangesAsync(cancellationToken );
        return new Output
        {
            isError = false,
            data = null,
            code = 200,
            message = "Xóa lớp học thành công."
        };
    }
}

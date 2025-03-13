using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public record EditLichHocCommand : IRequest<Output>
{
   public required EditlLichHocDto LichHocDto { get; init; }
}

public class EditLichHocCommandHandler : IRequestHandler<EditLichHocCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditLichHocCommandHandler(IApplicationDbContext context, IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(EditLichHocCommand request, CancellationToken cancellationToken)
    {
        var lichHocDto = request.LichHocDto;

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var coSoId =  _identityService.GetCampusId(token);
        var lichHoc = await _context.LichHocs.FindAsync(lichHocDto.Id)
            ?? throw new NotFoundDataException($"Không tìm thấy lịch học với ID {lichHocDto.Id}.");

        if (lichHocDto.PhongId.HasValue)
        {
            var phong = await _context.Phongs.FirstOrDefaultAsync(p => p.Id == lichHocDto.PhongId.Value && p.CoSoId == coSoId);
            if (phong == null)
                throw new NotFoundDataException("Phòng không tồn tại hoặc không thuộc cơ sở của bạn.");
        }

        if (lichHocDto.Thu.HasValue) lichHoc.Thu = lichHocDto.Thu.Value;
        if (lichHocDto.PhongId.HasValue) lichHoc.PhongId = lichHocDto.PhongId.Value;
        if (!string.IsNullOrWhiteSpace(lichHocDto.TenLop)) lichHoc.TenLop = lichHocDto.TenLop;

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = lichHoc,
            code = 200,
            message = "Cập nhật lịch học thành công."
        };
    }
}

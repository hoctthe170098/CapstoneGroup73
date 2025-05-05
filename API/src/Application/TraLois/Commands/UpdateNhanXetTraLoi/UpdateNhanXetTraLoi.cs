using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.TraLois.Commands.UpdateNhanXetTraLoi;
public record UpdateNhanXetTraLoiCommand : IRequest<Output>
{
    public Guid TraLoiId { get; init; }
    public float? Diem { get; init; }
    public string? NhanXet { get; init; }
}
public class UpdateNhanXetTraLoiCommandHandler : IRequestHandler<UpdateNhanXetTraLoiCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public UpdateNhanXetTraLoiCommandHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(UpdateNhanXetTraLoiCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var giaoVien = await _context.GiaoViens
            .AsNoTracking()
            .FirstOrDefaultAsync(gv => gv.UserId == userId, cancellationToken)
            ?? throw new Exception("Không tìm thấy giáo viên.");

        var traLoi = await _context.TraLois
            .Include(t => t.Baitap)
                .ThenInclude(bt => bt.LichHoc)
            .FirstOrDefaultAsync(t => t.Id == request.TraLoiId, cancellationToken)
            ?? throw new Exception("Không tìm thấy câu trả lời.");

        if (traLoi.Baitap.LichHoc.GiaoVienCode != giaoVien.Code)
            throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa nhận xét cho bài này.");

        traLoi.Diem = request.Diem;
        traLoi.NhanXet = request.NhanXet;

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            message = "Cập nhật nhận xét và điểm thành công.",
            data = new { traLoi.Id, traLoi.Diem, traLoi.NhanXet }
        };
    }
}


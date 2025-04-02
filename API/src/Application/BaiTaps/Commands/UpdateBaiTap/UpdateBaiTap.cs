using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;

public record UpdateBaiTapCommand : IRequest<Output>
{
    public required UpdateBaiTapDto UpdateBaiTapDto { get; init; }
}

public class UpdateBaiTapCommandHandler : IRequestHandler<UpdateBaiTapCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateBaiTapCommandHandler(IApplicationDbContext context,IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(UpdateBaiTapCommand request, CancellationToken cancellationToken)
    {
        var dto = request.UpdateBaiTapDto;

        var baiTap = await _context.BaiTaps
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (baiTap == null)
            throw new NotFoundDataException("Không tìm thấy bài tập.");

        baiTap.TieuDe = dto.TieuDe;
        baiTap.NoiDung = dto.NoiDung;
        baiTap.ThoiGianKetThuc = dto.ThoiGianKetThuc;
        baiTap.UrlFile = dto.UrlFile;
        baiTap.TrangThai = dto.TrangThai;

        await _context.SaveChangesAsync(cancellationToken);

        var giaoVienCode = GetCurrentUserCode();
        if (string.IsNullOrWhiteSpace(giaoVienCode))
            throw new UnauthorizedAccessException("Không xác định được giáo viên từ token.");

        var lichHocs = await _context.LichHocs
            .AsNoTracking()
            .Where(lh => lh.GiaoVienCode == giaoVienCode)
            .Select(lh => new LichHocDropdownDto
            {
                Id = lh.Id,
                TenLop = lh.TenLop
            })
            .ToListAsync(cancellationToken);

        var response = new
        {
            BaiTap = baiTap,
            LichHocs = lichHocs
        };

        return new Output
        {
            isError = false,
            code = 200,
            data = response,
            message = "Cập nhật bài tập thành công ."
        };
    }

    private string? GetCurrentUserCode()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user?.FindFirst("sub")?.Value;
    }
}

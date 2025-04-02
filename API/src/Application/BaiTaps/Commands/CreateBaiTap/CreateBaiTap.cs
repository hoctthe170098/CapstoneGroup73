using MediatR;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
public record CreateBaiTapCommand : IRequest<Output>
{
    public required CreateBaiTapDto CreateBaiTapDto { get; init; }
}

public class CreateBaiTapCommandHandler : IRequestHandler<CreateBaiTapCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateBaiTapCommandHandler(IApplicationDbContext context,IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(CreateBaiTapCommand request, CancellationToken cancellationToken)
    {
        var giaoVienCode = GetCurrentUserCode();
        if (string.IsNullOrWhiteSpace(giaoVienCode))
            throw new UnauthorizedAccessException("Không tìm thấy mã giáo viên trong token.");

        var dto = request.CreateBaiTapDto;

        var lichHoc = await _context.LichHocs
            .FirstOrDefaultAsync(lh =>
                lh.Id == dto.LichHocId &&
                lh.GiaoVienCode == giaoVienCode,
                cancellationToken);

        if (lichHoc == null)
            throw new Exception("Bạn không có quyền tạo bài tập cho lớp học này.");

        var baiTap = new BaiTap
        {
            Id = Guid.NewGuid(),
            NgayTao = DateOnly.FromDateTime(DateTime.UtcNow), 
            LichHocId = dto.LichHocId,
            TieuDe = dto.TieuDe,
            NoiDung = dto.NoiDung,
            ThoiGianKetThuc = dto.ThoiGianKetThuc,
            UrlFile = dto.UrlFile,
            TrangThai = dto.TrangThai
        };

        _context.BaiTaps.Add(baiTap);
        await _context.SaveChangesAsync(cancellationToken);

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
            message = "Tạo bài tập thành công"
        };
    }

    private string? GetCurrentUserCode()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user?.FindFirst("sub")?.Value;
    }
}

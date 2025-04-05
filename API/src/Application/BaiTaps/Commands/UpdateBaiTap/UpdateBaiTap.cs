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
    private readonly IIdentityService _identityService;

    public UpdateBaiTapCommandHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(UpdateBaiTapCommand request, CancellationToken cancellationToken)
    {
        //  Lấy token
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var giaoVienId = _identityService.GetUserId(token);
        var dto = request.UpdateBaiTapDto;

        //  Tìm bài tập và kiểm tra quyền sở hữu
        var baiTap = await _context.BaiTaps
            .Include(bt => bt.LichHoc)
            .ThenInclude(lh => lh.GiaoVien)
            .FirstOrDefaultAsync(bt => bt.Id == dto.Id, cancellationToken);

        if (baiTap == null)
            throw new NotFoundDataException("Không tìm thấy bài tập.");

        if (baiTap.LichHoc.GiaoVien.UserId != giaoVienId.ToString())
            throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa bài tập này.");

        //  Trạng thái chỉ được phép: "Đang mở" hoặc "Chưa mở"
        var allowedStatuses = new[] { "Đang mở", "Chưa mở" };
        if (!string.IsNullOrWhiteSpace(dto.TrangThai) &&
            !allowedStatuses.Contains(dto.TrangThai.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new WrongInputException("Trạng thái không hợp lệ. Chỉ được phép 'Đang mở' hoặc 'Chưa mở'.");
        }

        baiTap.TieuDe = dto.TieuDe;
        baiTap.NoiDung = dto.NoiDung;
        baiTap.ThoiGianKetThuc = dto.ThoiGianKetThuc;
        baiTap.UrlFile = dto.UrlFile;
        baiTap.TrangThai = dto.TrangThai;

        await _context.SaveChangesAsync(cancellationToken);

        var lichHocs = await _context.LichHocs
            .AsNoTracking()
            .Where(lh => lh.GiaoVien.UserId == giaoVienId.ToString())
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
            message = "Cập nhật bài tập thành công."
        };
    }
}

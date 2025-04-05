using MediatR;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;

public record CreateBaiTapCommand : IRequest<Output>
{
    public required CreateBaiTapDto CreateBaiTapDto { get; init; }
}

public class CreateBaiTapCommandHandler : IRequestHandler<CreateBaiTapCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public CreateBaiTapCommandHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(CreateBaiTapCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ Authorization header
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var giaoVienId = _identityService.GetUserId(token);

        var dto = request.CreateBaiTapDto;

        var lichHoc = await _context.LichHocs
            .Include(lh => lh.GiaoVien)
            .FirstOrDefaultAsync(lh =>
                lh.Id == dto.LichHocId &&
                lh.GiaoVien.UserId == giaoVienId.ToString(),
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
            TrangThai = "Chưa mở"
        };

        _context.BaiTaps.Add(baiTap);
        await _context.SaveChangesAsync(cancellationToken);

        // Truy vấn danh sách lịch học để trả về cho dropdown
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
            message = "Tạo bài tập thành công"
        };
    }
}

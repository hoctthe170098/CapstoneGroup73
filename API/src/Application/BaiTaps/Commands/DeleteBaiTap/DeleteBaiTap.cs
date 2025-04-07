using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.BaiTaps.Commands.DeleteBaiTap;

public record DeleteBaiTapCommand(Guid Id) : IRequest<Output>;

public class DeleteBaiTapCommandHandler : IRequestHandler<DeleteBaiTapCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<DeleteBaiTapCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public DeleteBaiTapCommandHandler(
        IApplicationDbContext context,
        IWebHostEnvironment webHostEnvironment,
        ILogger<DeleteBaiTapCommandHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(DeleteBaiTapCommand request, CancellationToken cancellationToken)
    {
        // Lấy token và xác thực
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        // Lấy mã giáo viên từ UserId
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == userId, cancellationToken);

        if (giaoVien == null)
            throw new NotFoundDataException("Không tìm thấy giáo viên tương ứng.");

        var giaoVienCode = giaoVien.Code;

        // Tìm bài tập thuộc về giáo viên (theo code)
        var baiTap = await _context.BaiTaps
            .Include(bt => bt.LichHoc)
            .FirstOrDefaultAsync(bt =>
                bt.Id == request.Id &&
                bt.LichHoc.GiaoVienCode == giaoVienCode,
                cancellationToken);

        if (baiTap == null)
            throw new NotFoundDataException("Không tìm thấy bài tập hoặc bạn không có quyền xoá.");

        // Xoá file nếu có
        DeleteFile(baiTap.UrlFile);

        _context.BaiTaps.Remove(baiTap);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = null,
            message = "Xoá bài tập thành công."
        };
    }

    private void DeleteFile(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return;

        try
        {
            var fullPath = Path.Combine(_webHostEnvironment.ContentRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Xoá file thất bại: {FilePath}", filePath);
        }
    }
}

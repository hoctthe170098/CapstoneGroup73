using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.TraLois.Commands.DeleteTraLoi;

public record DeleteTraLoiCommand(Guid TraLoiId) : IRequest<Output>;

public class DeleteTraLoiCommandHandler : IRequestHandler<DeleteTraLoiCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DeleteTraLoiCommandHandler> _logger;

    public DeleteTraLoiCommandHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,
        IWebHostEnvironment env,
        ILogger<DeleteTraLoiCommandHandler> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _env = env;
        _logger = logger;
    }

    public async Task<Output> Handle(DeleteTraLoiCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .FirstOrDefaultAsync(hs => hs.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var traLoi = await _context.TraLois
            .FirstOrDefaultAsync(t => t.Id == request.TraLoiId && t.HocSinhCode == hocSinh.Code, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy câu trả lời hoặc bạn không có quyền xóa.");

        // Xóa file đính kèm nếu có
        if (!string.IsNullOrEmpty(traLoi.UrlFile))
        {
            DeleteFile(traLoi.UrlFile);
        }

        _context.TraLois.Remove(traLoi);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            message = "Xóa câu trả lời thành công"
        };
    }

    private void DeleteFile(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        try
        {
            var fullPath = Path.Combine(_env.ContentRootPath, url.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa file {Url}", url);
        }
    }
}

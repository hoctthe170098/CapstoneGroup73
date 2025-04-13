using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using System.IO;

namespace StudyFlow.Application.TraLois.Commands.UpdateTraLoi;

public record UpdateTraLoiCommand : IRequest<Output>
{
    public required Guid TraLoiId { get; init; }
    public required string NoiDung { get; init; }
    public IFormFile? TepDinhKemMoi { get; init; }
}

public class UpdateTraLoiCommandHandler : IRequestHandler<UpdateTraLoiCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UpdateTraLoiCommandHandler> _logger;

    public UpdateTraLoiCommandHandler(IApplicationDbContext context,IHttpContextAccessor httpContextAccessor,IIdentityService identityService,IWebHostEnvironment env,ILogger<UpdateTraLoiCommandHandler> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _env = env;
        _logger = logger;
    }

    public async Task<Output> Handle(UpdateTraLoiCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ.");

        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(hs => hs.UserId == userId, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var traLoi = await _context.TraLois
            .FirstOrDefaultAsync(t => t.Id == request.TraLoiId && t.HocSinhCode == hocSinh.Code, cancellationToken)
            ?? throw new NotFoundDataException("Không tìm thấy trả lời hoặc bạn không có quyền cập nhật.");

        // Cập nhật nội dung
        traLoi.NoiDung = request.NoiDung;
        traLoi.ThoiGian = DateTime.Now;

        // Xử lý file đính kèm
        if (request.TepDinhKemMoi != null)
        {
            // Có file mới xóa file cũ và cập nhật
            if (!string.IsNullOrEmpty(traLoi.UrlFile))
                DeleteFile(traLoi.UrlFile);

            traLoi.UrlFile = await UploadFile(request.TepDinhKemMoi, cancellationToken);
        }
        
        

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = traLoi,
            message = "Cập nhật trả lời thành công"
        };
    }

    private async Task<string> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var folder = Path.Combine(_env.ContentRootPath, "traloi_files");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return "/traloi_files/" + fileName;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Lỗi upload file: {File}", file.FileName);
            throw new Exception($"Upload file thất bại: {ex.Message}");
        }
    }

    private void DeleteFile(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return;

        try
        {
            var fullPath = Path.Combine(_env.ContentRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Xoá file cũ lỗi: {FilePath}", filePath);
        }
    }
}

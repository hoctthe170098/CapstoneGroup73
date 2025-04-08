using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Models;
using System.Net.Mime;

namespace StudyFlow.Application.BaiTaps.Commands.DownloadBaiTap;

public record DownloadBaiTapCommand : IRequest<Output>
{
    public required string FilePath { get; init; }
}

public class DownloadBaiTapCommandHandler : IRequestHandler<DownloadBaiTapCommand, Output>
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<DownloadBaiTapCommandHandler> _logger;

    public DownloadBaiTapCommandHandler(
        IWebHostEnvironment webHostEnvironment,
        ILogger<DownloadBaiTapCommandHandler> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<Output> Handle(DownloadBaiTapCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, request.FilePath.TrimStart('/'));

            if (!File.Exists(filePath))
            {
                return new Output
                {
                    isError = true,
                    code = 404,
                    message = "Không tìm thấy file."
                };
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var fileName = Path.GetFileName(request.FilePath);
            var contentType = GetContentType(fileName);

            return new Output
            {
                isError = false,
                data = new FileContentResult(fileBytes, contentType)
                {
                    FileDownloadName = fileName
                },
                message = "Tải tệp thành công."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tải file: {FilePath}", request.FilePath);
            return new Output
            {
                isError = true,
                code = 500,
                message = "Đã xảy ra lỗi khi tải file."
            };
        }
    }

    private string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".mp4" => "video/mp4",
            _ => MediaTypeNames.Application.Octet
        };
    }
}

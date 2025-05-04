using Microsoft.AspNetCore.Hosting;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.IO; // Thêm using cho Path

namespace StudyFlow.Application.ChuongTrinhs.Commands.DownBaiThi;

public record DownBaiThiCommand : IRequest<Output>
{
    public required string filePath { get; init; }
}

public class DownBaiThiCommandHandler : IRequestHandler<DownBaiThiCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _uploadFolderPath;
    private readonly ILogger<DownBaiThiCommandHandler> _logger;

    public DownBaiThiCommandHandler(IApplicationDbContext context, IWebHostEnvironment webHostEnvironment
        , ILogger<DownBaiThiCommandHandler> logger)
    {
        _context = Guard.Against.Null(context);
        _webHostEnvironment = Guard.Against.Null(webHostEnvironment);
        _logger = Guard.Against.Null(logger);
        _uploadFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "baikiemtras");
        if (!Directory.Exists(_uploadFolderPath))
        {
            Directory.CreateDirectory(_uploadFolderPath);
        }
    }

    public async Task<Output> Handle(DownBaiThiCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, request.filePath.TrimStart('/')); // Sử dụng WebRootPath
            if (!File.Exists(filePath))
            {
                return new Output
                {
                    isError = true,
                    message = "File not found."
                };
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var fileName = Path.GetFileName(request.filePath);
            var contentType = GetContentType(fileName); // Lấy Content-Type từ hàm GetContentType

            return new Output
            {
                isError = false,
                data = new FileContentResult(fileBytes, contentType)
                {
                    FileDownloadName = fileName
                },
                message = "File downloaded successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file.");
            return new Output
            {
                isError = true,
                message = "An error occurred while downloading the file."
            };
        }
    }

    private string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        switch (ext)
        {
            case ".pdf":
                return "application/pdf";
            case ".doc":
                return "application/msword";
            case ".docx":
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            case ".mp4":
                return "video/mp4";
            default:
                return MediaTypeNames.Application.Octet; // Mặc định cho các loại file khác
        }
    }
}

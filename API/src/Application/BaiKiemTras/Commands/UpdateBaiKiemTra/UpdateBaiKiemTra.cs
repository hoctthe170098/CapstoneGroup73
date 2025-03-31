using Microsoft.AspNetCore.Hosting;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateBaiKiemTra;

public record UpdateBaiKiemTraCommand : IRequest<Output>
{
    public required UpdateBaiKiemTraDto BaiKiemTraDto { get; init; }
}

public class UpdateBaiKiemTraCommandHandler : IRequestHandler<UpdateBaiKiemTraCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _baikiemtraFolderPath;
    private readonly ILogger<UpdateBaiKiemTraCommandHandler> _logger;

    public UpdateBaiKiemTraCommandHandler(IApplicationDbContext context
        , IWebHostEnvironment webHostEnvironment
        , ILogger<UpdateBaiKiemTraCommandHandler> logger)
    {
        _context = Guard.Against.Null(context);
        _webHostEnvironment = Guard.Against.Null(webHostEnvironment);
        _logger = Guard.Against.Null(logger);
        _baikiemtraFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "baikiemtras");
        if (!Directory.Exists(_baikiemtraFolderPath))
        {
            Directory.CreateDirectory(_baikiemtraFolderPath);
        }
    }

    public async Task<Output> Handle(UpdateBaiKiemTraCommand request, CancellationToken cancellationToken)
    {
        BaiKiemTra baiKiemTra = await _context.BaiKiemTras
            .Where(b=>b.Id==request.BaiKiemTraDto.Id)
            .FirstAsync();
        baiKiemTra.Ten = request.BaiKiemTraDto.TenBaiKiemTra;
        baiKiemTra.NgayKiemTra = request.BaiKiemTraDto.NgayKiemTra;
        if (request.BaiKiemTraDto.TaiLieu != null)
        {
            DeleteFile(baiKiemTra.UrlFile);
            string urlTaiLieu = await UploadDeThi(request.BaiKiemTraDto.TaiLieu, cancellationToken);
            baiKiemTra.UrlFile = urlTaiLieu;
        }
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            code = 200,
            data = baiKiemTra,
            message = "Chỉnh sửa bài kiểm tra thành công"
        };
    }
    private async Task<string> UploadDeThi(IFormFile deThi, CancellationToken cancellationToken)
    {
            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(deThi.FileName);
                var filePath = Path.Combine(_baikiemtraFolderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await deThi.CopyToAsync(stream, cancellationToken);
                }
                return "/baikiemtras/" + fileName;        
            }
            catch (IOException ioException)
            {
                _logger.LogError(ioException, "Error uploading file {FileName}", deThi.FileName);
                throw new Exception($"Error uploading file {deThi.FileName}: {ioException.Message}");
            }
    }
    private void DeleteFile(string filePath)
    {
        try
        {
            var fullFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
        }
        catch (IOException ioException)
        {
            _logger.LogError(ioException, "Error deleting file {FilePath}", filePath);
            throw new Exception($"Error deleting file {filePath}: {ioException.Message}");
        }
    }
}

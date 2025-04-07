using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;

public record UpdateBaiTapCommand : IRequest<Output>
{
    public required UpdateBaiTapDto UpdateBaiTapDto { get; init; }
}

public class UpdateBaiTapCommandHandler : IRequestHandler<UpdateBaiTapCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<UpdateBaiTapCommandHandler> _logger;
    private readonly string _folderPath;

    public UpdateBaiTapCommandHandler(
        IApplicationDbContext context,
        IWebHostEnvironment webHostEnvironment,
        ILogger<UpdateBaiTapCommandHandler> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _folderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "baitaps");

        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }
    }

    public async Task<Output> Handle(UpdateBaiTapCommand request, CancellationToken cancellationToken)
    {
        var dto = request.UpdateBaiTapDto;

        var baiTap = await _context.BaiTaps
            .Include(bt => bt.LichHoc)
            .FirstOrDefaultAsync(bt => bt.Id == dto.Id, cancellationToken);

        if (baiTap == null)
            throw new NotFoundDataException("Không tìm thấy bài tập.");

        //  Validate trạng thái
        var allowedStatuses = new[] { "Đang mở", "Chưa mở" };
        if (!string.IsNullOrWhiteSpace(dto.TrangThai) &&
            !allowedStatuses.Contains(dto.TrangThai.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new WrongInputException("Trạng thái không hợp lệ. Chỉ cho phép 'Đang mở' hoặc 'Chưa mở'.");
        }

        //  Cập nhật thông tin bài tập
        baiTap.TieuDe = dto.TieuDe;
        baiTap.NoiDung = dto.NoiDung;
        baiTap.ThoiGianKetThuc = dto.ThoiGianKetThuc;
        baiTap.TrangThai = dto.TrangThai;

        //  Xử lý file nếu có upload mới
        if (dto.TaiLieu != null)
        {
            DeleteFile(baiTap.UrlFile); // xóa file cũ nếu có
            baiTap.UrlFile = await UploadFileAsync(dto.TaiLieu, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = baiTap,
            message = "Cập nhật bài tập thành công"
        };
    }

    private async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var savePath = Path.Combine(_folderPath, fileName);

            await using var stream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/baitaps/{fileName}";
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Lỗi khi upload file {FileName}", file.FileName);
            throw new Exception($"Không thể upload file {file.FileName}: {ex.Message}");
        }
    }

    private void DeleteFile(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        try
        {
            var fullPath = Path.Combine(_webHostEnvironment.ContentRootPath, url.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa file {Url}", url);
        }
    }
}

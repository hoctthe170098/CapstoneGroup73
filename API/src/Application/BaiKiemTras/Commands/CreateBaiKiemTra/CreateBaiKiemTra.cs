using Microsoft.AspNetCore.Hosting;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Ardalis.GuardClauses;
using StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateBaiKiemTra;

public record CreateBaiKiemTraCommand : IRequest<Output>
{
    public required UpdateBaiKiemTraDto BaiKiemTraDto { get; init; }
}

public class CreateBaiKiemTraCommandHandler : IRequestHandler<CreateBaiKiemTraCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _baikiemtraFolderPath;
    private readonly ILogger<CreateBaiKiemTraCommandHandler> _logger;

    public CreateBaiKiemTraCommandHandler(IApplicationDbContext context
        , IWebHostEnvironment webHostEnvironment
        , ILogger<CreateBaiKiemTraCommandHandler> logger)
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

    public async Task<Output> Handle(CreateBaiKiemTraCommand request, CancellationToken cancellationToken)
    {
        string urlTaiLieu = await UploadDeThi(request.BaiKiemTraDto.TaiLieu, cancellationToken);
        BaiKiemTra baiKiemTra = new BaiKiemTra
        {
            Id = Guid.NewGuid(),
            NgayTao = DateOnly.FromDateTime(DateTime.Now),
            Ten = request.BaiKiemTraDto.TenBaiKiemTra,
            TrangThai = "Chưa kiểm tra",
            UrlFile = urlTaiLieu,
            NgayKiemTra = request.BaiKiemTraDto.NgayKiemTra
        };
        var thu = ((int)request.BaiKiemTraDto.NgayKiemTra.DayOfWeek > 0)
            ? (int)request.BaiKiemTraDto.NgayKiemTra.DayOfWeek + 1
            : (int)request.BaiKiemTraDto.NgayKiemTra.DayOfWeek;
        var lichHoc = await _context.LichHocs
            .Where(lh => lh.TenLop == request.BaiKiemTraDto.TenLop
            && lh.Thu == thu && lh.TrangThai == "Cố định").FirstAsync();
        baiKiemTra.LichHocId = lichHoc.Id;
        await _context.BaiKiemTras.AddAsync(baiKiemTra, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            code = 200,
            data = baiKiemTra,
            message = "Thêm bài kiểm tra thành công"
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
}

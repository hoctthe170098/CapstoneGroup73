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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public CreateBaiKiemTraCommandHandler(IApplicationDbContext context
        , IWebHostEnvironment webHostEnvironment
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService
        , ILogger<CreateBaiKiemTraCommandHandler> logger)
    {
        _context = Guard.Against.Null(context);
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
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
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        string urlTaiLieu = await UploadDeThi(request.BaiKiemTraDto.TaiLieu, cancellationToken);
        BaiKiemTra baiKiemTra = new BaiKiemTra
        {
            Id = Guid.NewGuid(),
            NgayTao = DateOnly.FromDateTime(DateTime.Now),
            Ten = request.BaiKiemTraDto.TenBaiKiemTra,
            TrangThai = "Chưa kiểm tra",
            UrlFile = urlTaiLieu,
            NgayKiemTra = request.BaiKiemTraDto.NgayKiemTra,
            LichHocId = Guid.NewGuid()
        };
        var thu = ((int)request.BaiKiemTraDto.NgayKiemTra.DayOfWeek > 0)
            ? (int)request.BaiKiemTraDto.NgayKiemTra.DayOfWeek + 1
            : (int)request.BaiKiemTraDto.NgayKiemTra.DayOfWeek;
        var lichHoc = await _context.LichHocs
            .Where(lh => lh.TenLop == request.BaiKiemTraDto.TenLop
            && lh.Thu == thu && lh.TrangThai == "Cố định"&&lh.Phong.CoSoId==coSoId).FirstAsync();
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

using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using System.IO;

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
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<CreateBaiTapCommandHandler> _logger;
    private readonly string _baitapFolderPath;

    public CreateBaiTapCommandHandler(IApplicationDbContext context,IHttpContextAccessor httpContextAccessor,IIdentityService identityService,IWebHostEnvironment webHostEnvironment,ILogger<CreateBaiTapCommandHandler> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;

        _baitapFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "baitaps");
        if (!Directory.Exists(_baitapFolderPath))
        {
            Directory.CreateDirectory(_baitapFolderPath);
        }
    }

    public async Task<Output> Handle(CreateBaiTapCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var giaoVienId = _identityService.GetUserId(token);
        var dto = request.CreateBaiTapDto;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thuHienTai = (int)DateTime.UtcNow.DayOfWeek;
        thuHienTai = thuHienTai == 0 ? 8 : thuHienTai + 1;

        //  Lấy GiaoVienCode từ UserId
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == giaoVienId.ToString(), cancellationToken);

        if (giaoVien == null)
            throw new NotFoundDataException("Không tìm thấy giáo viên tương ứng.");

        var giaoVienCode = giaoVien.Code;

        //  Tìm lịch học hôm nay
        var lichHoc = await _context.LichHocs
            .FirstOrDefaultAsync(lh =>
                lh.TenLop == dto.TenLop &&
                lh.Thu == thuHienTai &&
                lh.GiaoVienCode == giaoVienCode &&
                lh.NgayBatDau <= today &&
                today <= lh.NgayKetThuc,
                cancellationToken);

        if (lichHoc == null)
            throw new NotFoundIDException();

        string urlFile = string.Empty;
        if (dto.TaiLieu != null)
        {
            urlFile = await UploadTaiLieu(dto.TaiLieu, cancellationToken);
        }

        var baiTap = new BaiTap
        {
            Id = Guid.NewGuid(),
            NgayTao = today,
            LichHocId = lichHoc.Id,
            TieuDe = dto.TieuDe,
            NoiDung = dto.NoiDung,
            ThoiGianKetThuc = dto.ThoiGianKetThuc,
            UrlFile = urlFile,
            TrangThai = "Chưa mở"
        };

        _context.BaiTaps.Add(baiTap);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = baiTap,
            message = "Tạo bài tập thành công"
        };
    }

    private async Task<string> UploadTaiLieu(IFormFile taiLieu, CancellationToken cancellationToken)
    {
        try
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(taiLieu.FileName);
            var filePath = Path.Combine(_baitapFolderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await taiLieu.CopyToAsync(stream, cancellationToken);

            return "/baitaps/" + fileName;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Lỗi khi upload tài liệu bài tập {FileName}", taiLieu.FileName);
            throw new Exception($"Không thể upload file {taiLieu.FileName}: {ex.Message}");
        }
    }
}

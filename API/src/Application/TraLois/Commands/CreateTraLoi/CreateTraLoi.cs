using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.TraLois.Commands.CreateTraLoi;

public record CreateTraLoiCommand : IRequest<Output>
{
    public Guid BaiTapId { get; init; }
    public string NoiDung { get; init; } = default!;
    public IFormFile? TepDinhKem { get; init; }
}

public class CreateTraLoiCommandHandler : IRequestHandler<CreateTraLoiCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CreateTraLoiCommandHandler> _logger;

    public CreateTraLoiCommandHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,
        IWebHostEnvironment env,
        ILogger<CreateTraLoiCommandHandler> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _env = env;
        _logger = logger;
    }

    public async Task<Output> Handle(CreateTraLoiCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

        var userId = _identityService.GetUserId(token).ToString();

        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken);

        if (hocSinh == null)
            throw new NotFoundDataException("Không tìm thấy học sinh tương ứng.");

        var hocSinhCode = hocSinh.Code;

        // Kiểm tra bài tập tồn tại 
        var baiTap = await _context.BaiTaps
            .Include(bt => bt.LichHoc)
            .FirstOrDefaultAsync(bt => bt.Id == request.BaiTapId, cancellationToken);

        if (baiTap == null)
            throw new NotFoundDataException("Không tìm thấy bài tập.");

        // ❗ Kiểm tra trạng thái bài tập
        if (baiTap.TrangThai == "Kết thúc")
            throw new InvalidOperationException("Bài tập đã kết thúc, bạn không thể gửi câu trả lời.");
        // Kiểm tra học sinh có học lớp này không
        var isThamGiaLop = await _context.ThamGiaLopHocs
            .AnyAsync(tg => tg.LichHocId == baiTap.LichHocId && tg.HocSinhCode == hocSinhCode&&tg.NgayKetThuc>=DateOnly.FromDateTime(baiTap.NgayTao), cancellationToken);
        if (!isThamGiaLop)
            throw new UnauthorizedAccessException("Bạn không có quyền trả lời bài tập này vì không thuộc lớp học tương ứng.");
        // Lưu file nếu có
        string? fileUrl = null;
        if (request.TepDinhKem != null)
        {
            fileUrl = await SaveFileAsync(request.TepDinhKem, cancellationToken);
        }

        var traLoi = new TraLoi
        {
            Id = Guid.NewGuid(),
            ThoiGian = DateTime.Now,
            NoiDung = request.NoiDung,
            UrlFile = fileUrl,
            HocSinhCode = hocSinhCode,
            BaiTapId = request.BaiTapId
        };

        _context.TraLois.Add(traLoi);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            message = "Gửi câu trả lời thành công",
            data = traLoi
        };
    }

    private async Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "tralois");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return "/tralois/" + fileName;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Lỗi khi lưu file trả lời: {FileName}", file.FileName);
            throw new Exception($"Không thể upload file {file.FileName}: {ex.Message}");
        }
    }
}

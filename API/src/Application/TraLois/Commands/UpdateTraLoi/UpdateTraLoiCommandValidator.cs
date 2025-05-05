using System.Threading;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.TraLois.Commands.UpdateTraLoi;

public class UpdateTraLoiCommandValidator : AbstractValidator<UpdateTraLoiCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public UpdateTraLoiCommandValidator(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung không được để trống.")
            .MaximumLength(750).WithMessage("Nội dung tối đa 750 ký tự.");

        RuleFor(x => x.TepDinhKemMoi)
            .Must(BeValidFile)
            .When(x => x.TepDinhKemMoi != null)
            .WithMessage("Tệp phải là .doc, .docx hoặc .pdf và không vượt quá 10MB.");

        RuleFor(x => x)
            .MustAsync(IsOwnerOfTraLoi)
            .WithMessage("Bạn không có quyền cập nhật câu trả lời này.");

        RuleFor(x => x)
            .MustAsync(IsBeforeDeadline)
            .WithMessage("Bài tập này đã hết hạn, bạn không thể sửa câu trả lời.");

        RuleFor(x => x)
            .MustAsync(ChuaDuocNhanXet)
            .WithMessage("Bài tập đã được nhận xét và chấm điểm, không thể sửa câu trả lời.");
    }

    private async Task<bool> ChuaDuocNhanXet(UpdateTraLoiCommand command, CancellationToken token)
    {
        var hocSinh = await GetHocSinhFromTokenAsync(token);
        if (hocSinh == null) return false;

        var traLoi = await _context.TraLois
        .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == command.TraLoiId && t.HocSinhCode == hocSinh.Code, token);
        if (traLoi?.Baitap == null) return false;
        if (traLoi.Diem != null || traLoi.NhanXet != null) return false;
        return true;
    }

    private bool BeValidFile(IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension) && file.Length <= 10 * 1024 * 1024;
    }

    private async Task<bool> IsOwnerOfTraLoi(UpdateTraLoiCommand command, CancellationToken cancellationToken)
    {
        var hocSinh = await GetHocSinhFromTokenAsync(cancellationToken);
        if (hocSinh == null) return false;

        return await _context.TraLois
            .AsNoTracking()
            .AnyAsync(t => t.Id == command.TraLoiId && t.HocSinhCode == hocSinh.Code, cancellationToken);
    }

    private async Task<bool> IsBeforeDeadline(UpdateTraLoiCommand command, CancellationToken cancellationToken)
    {
        var hocSinh = await GetHocSinhFromTokenAsync(cancellationToken);
        if (hocSinh == null) return false;

        var traLoi = await _context.TraLois
            .Include(t => t.Baitap)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == command.TraLoiId && t.HocSinhCode == hocSinh.Code, cancellationToken);

        if (traLoi?.Baitap == null) return false;

        var baiTap = traLoi.Baitap;

        if (baiTap.ThoiGianKetThuc.HasValue && DateTime.Now >= baiTap.ThoiGianKetThuc.Value)
            return false;

        if (baiTap.TrangThai?.Trim().Equals("Kết thúc", StringComparison.OrdinalIgnoreCase) == true)
            return false;

        return true;
    }

    private async Task<HocSinh?> GetHocSinhFromTokenAsync(CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token)) return null;

        var userId = _identityService.GetUserId(token).ToString();

        return await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken);
    }
}

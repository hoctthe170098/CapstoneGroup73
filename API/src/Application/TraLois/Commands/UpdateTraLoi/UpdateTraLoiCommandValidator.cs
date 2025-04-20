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
            .MustAsync(HocSinhSoHuuVaChuaHetHan)
            .WithMessage("Bạn không có quyền cập nhật câu trả lời này.");
        RuleFor(x => x)
            .MustAsync(BaiTapChuaHetHanAsync)
            .WithMessage("Bài tập này đã hết hạn bạn không thể sửa câu trả lời này");
    }

    private bool BeValidFile(IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var isValidSize = file.Length <= 10 * 1024 * 1024;

        return allowedExtensions.Contains(extension) && isValidSize;
    }

    private Task<bool> BaiTapChuaHetHanAsync(UpdateTraLoiCommand command, CancellationToken cancellationToken)
    {
        return HocSinhSoHuuVaChuaHetHan(command, cancellationToken);
    }

    private async Task<bool> HocSinhSoHuuVaChuaHetHan(UpdateTraLoiCommand command, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token)) return false;

        var userId = _identityService.GetUserId(token).ToString();
        var hocSinh = await _context.HocSinhs
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken);

        if (hocSinh == null) return false;

        var traLoi = await _context.TraLois
            .Include(t => t.Baitap)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == command.TraLoiId && t.HocSinhCode == hocSinh.Code, cancellationToken);

        if (traLoi == null) return false;

        var baiTap = traLoi.Baitap;

        if (baiTap.ThoiGianKetThuc.HasValue && DateTime.Now >= baiTap.ThoiGianKetThuc.Value)
            return false;

        if (baiTap.TrangThai?.Trim().Equals("Kết thúc", StringComparison.OrdinalIgnoreCase) == true)
            return false;

        return true;
    }

}

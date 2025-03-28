using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateBaiKiemTra;
public class UpdateBaiKiemTraCommandValidator : AbstractValidator<UpdateBaiKiemTraCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateBaiKiemTraCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.BaiKiemTraDto.Id)
            .MustAsync(ExistBaiKT1)
            .WithMessage("Bài kiểm tra này không thể chỉnh sửa được nữa.");
        RuleFor(x => x.BaiKiemTraDto.TenLop)
            .MustAsync(ExistBaiKT2)
            .WithMessage("Bài kiểm tra này không thể chỉnh sửa được nữa.");
        RuleFor(x => x.BaiKiemTraDto.TenBaiKiemTra)
            .MaximumLength(50).WithMessage("Tên bài kiểm tra tối đa 50 ký tự.")
            .MinimumLength(5).WithMessage("Tên bài kiểm tra tối thiểu 5 ký tự.");
        RuleFor(x => x.BaiKiemTraDto.NgayKiemTra)
            .MustAsync(ValidNgayKiemTra)
            .WithMessage("Ngày kiểm tra phải sau hôm nay ít nhất 7 ngày, " +
            "phải vào thứ mà học sinh học và phải nhỏ hơn ngày kết thúc khoá học.")
            .MustAsync(ValidLichKiemTraCoDinh)
            .WithMessage("Ngày kiểm tra phải vào lịch học cố định và giáo viên chính.")
            .MustAsync(NotDuplicatedLichThi)
            .WithMessage("Lớp học đã có bài kiểm tra trong ngày này.");
        RuleFor(x => x.BaiKiemTraDto.TaiLieu)
            .Must(ValidTaiLieu)
            .WithMessage("Đề thi phải là word hoặc pdf và độ lớn không quá 10Mb.");
    }

    private async Task<bool> ExistBaiKT2(UpdateBaiKiemTraCommand command, 
        string tenLop, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.BaiKiemTras
            .AnyAsync(b => b.Id == command.BaiKiemTraDto.Id 
            && b.LichHoc.Phong.CoSoId == coSoId
            && b.LichHoc.TenLop == tenLop
            && b.TrangThai== "Chưa kiểm tra");
    }

    private async Task<bool> ExistBaiKT1(UpdateBaiKiemTraCommand command, 
        Guid id, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
    .ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.BaiKiemTras
            .AnyAsync(b=>b.Id == id
            && b.LichHoc.Phong.CoSoId == coSoId
            && b.LichHoc.TenLop==command.BaiKiemTraDto.TenLop
            && b.TrangThai == "Chưa kiểm tra");
    }

    private async Task<bool> NotDuplicatedLichThi(UpdateBaiKiemTraCommand command,
        DateOnly ngayKiemTra, CancellationToken token)
    {
        var baiKiemTra = await _context.BaiKiemTras.FirstAsync(b => b.Id == command.BaiKiemTraDto.Id);
        if (ngayKiemTra == baiKiemTra.NgayKiemTra) return true;
        return !await _context.BaiKiemTras.AnyAsync(b => b.NgayKiemTra == ngayKiemTra
        && b.LichHoc.TenLop == command.BaiKiemTraDto.TenLop);
    }

    private async Task<bool> ValidLichKiemTraCoDinh(UpdateBaiKiemTraCommand command
        , DateOnly ngayKiemTra, CancellationToken token)
    {
        var baiKiemTra = await _context.BaiKiemTras.FirstAsync(b => b.Id == command.BaiKiemTraDto.Id);
        if (ngayKiemTra == baiKiemTra.NgayKiemTra) return true;
        var thu = ((int)ngayKiemTra.DayOfWeek > 0) ? (int)ngayKiemTra.DayOfWeek + 1 : (int)ngayKiemTra.DayOfWeek + 8;
        return !await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.BaiKiemTraDto.TenLop
        && lh.Thu == thu && lh.NgayKetThuc == ngayKiemTra 
        && (lh.TrangThai == "Dạy bù"||lh.TrangThai=="Dạy thay"));
    }

    private bool ValidTaiLieu(IFormFile? file)
    {
        if (file == null) return true;
        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (fileExtension != ".doc" && fileExtension != ".docx" && fileExtension != ".pdf")
        {
            return false; // File không phải word hoặc pdf
        }

        if (file.Length > 10 * 1024 * 1024) // 10MB
        {
            return false; // File vượt quá 10MB
        }
        return true; // File hợp lệ
    }

    private async Task<bool> ValidNgayKiemTra(UpdateBaiKiemTraCommand command,DateOnly ngayKiemTra, CancellationToken token)
    {
        var baiKiemTra = await _context.BaiKiemTras.FirstAsync(b => b.Id == command.BaiKiemTraDto.Id);
        if (ngayKiemTra == baiKiemTra.NgayKiemTra) return true;
        if(ngayKiemTra < DateOnly.FromDateTime(DateTime.Now).AddDays(7)) return false;
        var thu = ((int)ngayKiemTra.DayOfWeek >0) ? (int)ngayKiemTra.DayOfWeek + 1 : (int)ngayKiemTra.DayOfWeek + 8;
        return await _context.LichHocs.AnyAsync(lh => lh.TenLop == command.BaiKiemTraDto.TenLop
        && lh.Thu == thu&&lh.NgayKetThuc>=ngayKiemTra&&lh.TrangThai=="Cố định");
    }
}

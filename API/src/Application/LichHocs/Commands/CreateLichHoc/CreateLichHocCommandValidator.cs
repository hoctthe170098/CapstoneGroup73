using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;

public class GetLopHocWithPaginationQueryValidator : AbstractValidator<CreateLichHocCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public GetLopHocWithPaginationQueryValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.LopHocDto.TenLop)
            .NotEmpty().WithMessage("Tên lớp không được để trống")
            .MaximumLength(20).WithMessage("Tên lớp tối đa 20 ký tự")
            .MustAsync(NotExistName)
            .WithMessage("Tên lớp đã tồn tại");

        RuleFor(x => x.LopHocDto.NgayBatDau)
            .Must(BeValidDateOnly).WithMessage("Ngày bắt đầu không đúng định dạng DateOnly")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Ngày bắt đầu phải lớn hơn ngày hiện tại");

        RuleFor(x => x.LopHocDto.NgayKetThuc)
            .Must(BeValidDateOnly).WithMessage("Ngày kết thúc không đúng định dạng DateOnly")
            .GreaterThan(x => x.LopHocDto.NgayBatDau.AddMonths(2)).WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu ít nhất 2 tháng");

        RuleFor(x => x.LopHocDto.HocPhi)
            .GreaterThan(400000).WithMessage("Học phí phải lớn hơn 400,000");

        RuleFor(x => x.LopHocDto.GiaoVienCode)
            .NotEmpty().WithMessage("GiaoVienCode không được để trống")
            .MustAsync(ExistGiaoVien)
            .WithMessage("Giáo viên không tồn tại");

        RuleFor(x => x.LopHocDto.ChuongTrinhId)
            .GreaterThan(0).WithMessage("ChuongTrinhId phải là số nguyên dương")
            .MustAsync(ExistChuongTrinh)
            .WithMessage("Chương trình không tồn tại");

        RuleFor(x => x.LopHocDto.LichHocs)
            .NotEmpty().WithMessage("Danh sách Lịch Học không được để trống")
            .MustAsync(BeValidLichHocs).WithMessage("Danh sách Lịch Học đã bị trùng hoặc không hợp lệ");
    }

    private async Task<bool> NotExistName(string name, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return !await _context.LichHocs.AnyAsync(l=>l.TenLop==name&&l.Phong.CoSoId==coSoId,cToken);
    }

    private async Task<bool> ExistChuongTrinh(int chuongTrinhId, CancellationToken token)
    {
        return await _context.ChuongTrinhs.AnyAsync(ct => ct.Id == chuongTrinhId);
    }

    private async Task<bool> ExistGiaoVien(string code, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.GiaoViens.AnyAsync(gv => gv.Code == code && gv.CoSoId == coSoId);
    }

    private async Task<bool> ExistPhong(int phongId, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.Phongs.AnyAsync(p => p.Id == phongId&&p.CoSoId==coSoId&&p.CoSo.TrangThai=="use");
    }

    private bool BeValidDateOnly(DateOnly date)
    {
        return date != default; // Kiểm tra xem DateOnly có giá trị mặc định hay không
    }

    private bool BeValidTimeOnly(TimeOnly time)
    {
        return time != default; // Kiểm tra xem TimeOnly có giá trị mặc định hay không
    }

    private async Task<bool> BeValidLichHocs(CreateLichHocCommand command, List<LichHocDto> lichHocs, CancellationToken token)
    {
        if (lichHocs == null || lichHocs.Count == 0)
        {
            return true; // Danh sách rỗng hoặc null thì không cần validate
        }

        foreach (var lichHoc in lichHocs)
        {
            if(lichHoc.Thu<2||lichHoc.Thu>8) return false;
            var check = await ExistPhong(lichHoc.PhongId, token);
            if (!check) return false;

            // Kiểm tra GioBatDau định dạng TimeOnly
            if (!BeValidTimeOnly(lichHoc.GioBatDau) || lichHoc.GioBatDau < new TimeOnly(8, 0) || lichHoc.GioBatDau > new TimeOnly(20, 0))
            {
                return false;
            }

            // Kiểm tra GioKetThuc định dạng TimeOnly
            if (!BeValidTimeOnly(lichHoc.GioKetThuc) || lichHoc.GioKetThuc < new TimeOnly(10, 0) || lichHoc.GioKetThuc > new TimeOnly(22, 0))
            {
                return false;
            }

            if (lichHoc.GioKetThuc < lichHoc.GioBatDau.AddHours(2))
            {
                return false;
            }
        }

        // Kiểm tra trùng lịch học trong cùng phòng
        for (int i = 0; i < lichHocs.Count; i++)
        {
            for (int j = i + 1; j < lichHocs.Count; j++)
            {
                if (lichHocs[i].Thu==lichHocs[j].Thu)return false;
                if (lichHocs[i].PhongId == lichHocs[j].PhongId)
                {
                    if (!(lichHocs[i].GioKetThuc <= lichHocs[j].GioBatDau || lichHocs[j].GioKetThuc <= lichHocs[i].GioBatDau))
                    {
                        return false;
                    }
                    var timeSpan = (lichHocs[j].GioBatDau > lichHocs[i].GioKetThuc)
                        ? lichHocs[j].GioBatDau - lichHocs[i].GioKetThuc
                        : lichHocs[i].GioBatDau - lichHocs[j].GioKetThuc;
                    if (timeSpan < TimeSpan.FromMinutes(15)) return false;
                }
            }
        }
        var lichHocData = await _context.LichHocs
            .Select(l => new {l.GiaoVienCode,l.PhongId,l.GioBatDau,l.GioKetThuc,l.Thu})
            .ToListAsync();
        foreach (var lichHoc in lichHocs)
        {
            var checkThuAndPhong = lichHocData
                .Any(lh=>lh.Thu==lichHoc.Thu&&lh.PhongId==lichHoc.PhongId
                &&!(lh.GioKetThuc<=lichHoc.GioBatDau.AddMinutes(15)
                ||lh.GioBatDau>=lichHoc.GioKetThuc.AddMinutes(15)));
            if (!checkThuAndPhong) return false;
            var checkGiaoVien = lichHocData
                .Any(lh => lh.GiaoVienCode == command.LopHocDto.GiaoVienCode
                && !(lh.GioKetThuc <= lichHoc.GioBatDau.AddMinutes(15)
                || lh.GioBatDau >= lichHoc.GioKetThuc.AddMinutes(15)));
            if (!checkGiaoVien) return false;
        }
        return true;
    }
}

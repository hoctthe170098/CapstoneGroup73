using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;

public class GetLopHocWithPaginationQueryValidator : AbstractValidator<GetLopHocWithPaginationQuery>
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
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Trang không hợp lệ");
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Độ lớn trang không hợp lệ");
        RuleFor(x => x.TenLop)
            .NotNull()
            .WithMessage("Thiếu tên lớp.");
        RuleFor(x => x.Thus)
            .Must(BeValidThus)
            .WithMessage("Danh sách thứ không hợp lệ.");
        RuleFor(x => x.ChuongTrinhId)
            .GreaterThanOrEqualTo(0).WithMessage("ChuongTrinhId phải là số nguyên lớn hơn 0.")
            .MustAsync(ExistChuongTrinh)
            .WithMessage("Chương trình không tồn tại.");
        RuleFor(x => x.TrangThai)
            .Must(ExistTrangThai)
            .WithMessage("Trạng thái không hợp lệ.");
        RuleFor(x => x.GiaoVienCode)
            .NotNull().WithMessage("Thiếu code của giáo viên.")
            .MustAsync(ExistGiaoVien)
            .WithMessage("Giáo viên không tồn tại.");
        RuleFor(x => x.PhongId)
            .MustAsync(ExistPhong)
            .WithMessage("Phòng không tồn tại.");
        RuleFor(x => x.ThoiGianBatDau)
            .Must(ValidFormatTimeOnly)
            .WithMessage("Thời gian bắt đầu không hợp lệ");
        RuleFor(x => x.ThoiGianKetThuc)
            .Must(ValidFormatTimeOnlyKetThuc)
            .WithMessage("Thời gian kết thúc không hợp lệ");
        RuleFor(x => x.NgayKetThuc)
            .Must(ValidNgayKetThuc)
            .WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
    }

    private bool ValidNgayKetThuc(GetLopHocWithPaginationQuery query,DateOnly ngayKetThuc)
    {
        if (query.NgayBatDau == DateOnly.MinValue||ngayKetThuc==DateOnly.MinValue) return true;
        else
        {
            if(ngayKetThuc<query.NgayBatDau)return false;
        }
        return true;
    }

    private bool ValidFormatTimeOnlyKetThuc(GetLopHocWithPaginationQuery query,string thoiGianKetThuc)
    {
        if (thoiGianKetThuc.Trim() == "") return true;
        else
        {
            var checkFormat = TimeOnly.TryParse(thoiGianKetThuc, out var ketThuc);
            if(!checkFormat)return false;
        }
        return true;
    }

    private bool ValidFormatTimeOnly(string thoiGianBatDau)
    {
        if (thoiGianBatDau.Trim() == "") return true;
        else
        {
            var check = TimeOnly.TryParse(thoiGianBatDau,out var tg);
            if(!check) return false;
        }
        return true;
    }

    private bool ExistTrangThai(string trangThai)
    {
        trangThai = trangThai.ToLower().Trim();
        if(trangThai!="all"&&trangThai!="cố định" && trangThai!="dạy thay" && trangThai!="dạy bù") return false;
        return true;
    }

    private bool BeValidThus(List<int> list)
    {
        if (list == null) return true;
        var listThuNotDuplicate = list.Distinct().ToList();
        if (listThuNotDuplicate.Count!=list.Count) return false;
        foreach(var thu in list)
        {
            if (thu<2||thu>8)return false;
        }
        return true;
    }

    private async Task<bool> ExistChuongTrinh(int chuongTrinhId, CancellationToken token)
    {
        if (chuongTrinhId == 0) return true;
        return await _context.ChuongTrinhs.AnyAsync(ct => ct.Id == chuongTrinhId);
    }

    private async Task<bool> ExistGiaoVien(string code, CancellationToken cToken)
    {
        if (code.ToLower().Trim() == "all") return true;
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.GiaoViens.AnyAsync(gv => gv.Code == code && gv.CoSoId == coSoId);
    }

    private async Task<bool> ExistPhong(int phongId, CancellationToken cToken)
    {
        if(phongId == 0) return true;
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.Phongs.AnyAsync(p => p.Id == phongId&&p.CoSoId==coSoId&&p.TrangThai=="use");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;

namespace StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
public class GetBaiKiemTrasWithPaginationQueryValidator : AbstractValidator<GetBaiKiemTrasWithPaginationQuery>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public GetBaiKiemTrasWithPaginationQueryValidator(IApplicationDbContext context
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
        RuleFor(x => x.TrangThai)
            .Must(ValidTrangThai)
            .WithMessage("Trạng thái không tồn tại");
        RuleFor(x => x.TenLop)
            .MustAsync(ValidTenLop)
            .WithMessage("Tên lớp không tồn tại");
    }
    private async Task<bool> ValidTenLop(string tenLop, CancellationToken cToken)
    {
        if (tenLop.ToLower().Trim() == "all") return true;
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.LichHocs.AnyAsync(lh => lh.TenLop == tenLop && lh.Phong.CoSoId == coSoId);
    }
    private bool ValidTrangThai(string trangThai)
    {
        trangThai = trangThai.ToLower().Trim();
        if(trangThai!="all"&&trangThai!="đã kiểm tra"&&trangThai!="chưa kiểm tra") return false;
        return true;
    }
}

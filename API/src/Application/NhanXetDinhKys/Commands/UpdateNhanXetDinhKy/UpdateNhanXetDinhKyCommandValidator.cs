using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace StudyFlow.Application.NhanXetDinhKys.Commands.UpdateNhanXetDinhKy;

public class UpdateNhanXetDinhKyCommandValidator : AbstractValidator<UpdateNhanXetDinhKyCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public UpdateNhanXetDinhKyCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.Id)
            .MustAsync(TonTaiNhanXetDinhKy)
            .WithMessage("Nhận xét không tồn tại hoặc không thuộc quyền chỉnh sửa của bạn");
        RuleFor(x => x.NoiDungNhanXet)
            .Must(DungFormat)
            .WithMessage("Nội dung nhận xét không được để trống và phải ít hơn 200 ký tự");
    }
    private async Task<bool> TonTaiNhanXetDinhKy(Guid id, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var UserId = _identityService.GetUserId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == UserId.ToString());
        if (giaoVien == null) throw new NotFoundIDException();
        return await _context.NhanXetDinhKys
            .AnyAsync(x => x.Id == id&&x.ThamGiaLopHoc.LichHoc.GiaoVienCode==giaoVien.Code);
    }
    private bool DungFormat(string noiDung)
    {
        noiDung = noiDung.Trim();
        return (noiDung.Length > 0 && noiDung.Length <= 200);
    }
}

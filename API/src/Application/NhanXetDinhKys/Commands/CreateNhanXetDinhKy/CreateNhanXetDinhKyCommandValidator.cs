using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace StudyFlow.Application.NhanXetDinhKys.Commands.CreateNhanXetDinhKy;

public class CreateNhanXetDinhKyCommandValidator : AbstractValidator<CreateNhanXetDinhKyCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public CreateNhanXetDinhKyCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.HocSinhCode)
            .MustAsync(TonTaiHocSinh)
            .WithMessage("Học sinh không tồn tại trong cơ sở này.");
        RuleFor(x => x.TenLop)
            .MustAsync(TonTaiLopHoc)
            .WithMessage("Lớp học không tồn tại trong cơ sở này.");
        RuleFor(x => x.NoiDungNhanXet)
            .Must(DungFormat)
            .WithMessage("Nội dung nhận xét không được để trống và phải ít hơn 200 ký tự");
    }

    private bool DungFormat(string noiDung)
    {
        noiDung = noiDung.Trim();
        return (noiDung.Length>0&&noiDung.Length<=200);
    }

    private async Task<bool> TonTaiLopHoc(string tenLop, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.LichHocs.AnyAsync(lh => lh.TenLop == tenLop && lh.Phong.CoSoId == coSoId);
    }

    private async Task<bool> TonTaiHocSinh(string code, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.HocSinhs.AnyAsync(hs=>hs.Code==code&&hs.CoSoId==coSoId);
    }
}

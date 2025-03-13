using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
public record EditGiaoVienCommand : IRequest<Output>
{
    public required string Code { get; init; }
    public required string Ten { get; init; }
    public required string GioiTinh { get; init; }
    public required string DiaChi { get; init; }
    public required string TruongDangDay { get; init; }
    public required string NgaySinh { get; init; }
    public required string Email { get; init; }
    public required string SoDienThoai { get; init; }
    public required string Status {  get; init; }
}

public class EditGiaoVienCommandHandler : IRequestHandler<EditGiaoVienCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditGiaoVienCommandHandler(IApplicationDbContext context
        , IIdentityService identityService
        , IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(EditGiaoVienCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ request header
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.DiaChi) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.TruongDangDay)||
            string.IsNullOrWhiteSpace(request.Status)) 
        {
            throw new NotFoundDataException("Dữ liệu không được để trống");
        }
        //Validate Status
        if(request.Status.ToLower()!="true"&&request.Status.ToLower()!="false")
            throw new FormatException("Trạng thái không hợp lệ" +
                ", chỉ có thể là true hoặc false");
        // Validate NgaySinh (Date of Birth) format
        if (!DateOnly.TryParseExact(request.NgaySinh, "yyyy-MM-dd", out DateOnly ngaySinh))
        {
            throw new FormatException("Ngày sinh không hợp lệ. Định dạng phải là yyyy-MM-dd");
        }
        // Validate NgaySinh (Date of Birth) is at least 18 years old
        var eighteenYearsAgo = DateOnly.FromDateTime(DateTime.Now.AddYears(-18));
        if (ngaySinh > eighteenYearsAgo)
        {
            throw new WrongInputException("Giáo viên phải đủ 18 tuổi trở lên");
        }
        // Validate length constraints
        if (request.Code.Length > 20 || request.Ten.Length > 50 || request.DiaChi.Length > 100)
        {
            throw new WrongInputException("Độ dài dữ liệu không hợp lệ!");
        }
        Guid coSoId = _identityService.GetCampusId(token);
        if (coSoId == Guid.Empty)
        {
            throw new FormatException("CoSo không hợp lệ");
        }
        // Validate phone number 
        if (!string.IsNullOrEmpty(request.SoDienThoai))
        {
            if (request.SoDienThoai.Length > 11 || !request.SoDienThoai.StartsWith("0") || !request.SoDienThoai.All(char.IsDigit))
            {
                throw new FormatException("Số điện thoại không hợp lệ");
            }
        }
        // Validate email 
        if (!string.IsNullOrEmpty(request.Email) && !new EmailAddressAttribute()
            .IsValid(request.Email))
        {
            throw new FormatException("Email không hợp lệ");
        }
        // Check If phone or email exist
        var phoneExists = await _context.GiaoViens
            .AnyAsync(nv => nv.SoDienThoai == request.SoDienThoai, cancellationToken);
        var emailExists = await _context.GiaoViens
            .AnyAsync(nv => nv.Email == request.Email, cancellationToken);
        if (phoneExists || emailExists)
        {
            throw new WrongInputException($"Số điện thoại hoặc email đã tồn tại");
        }
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv=>gv.Code==request.Code
        &&gv.CoSoId==coSoId, cancellationToken);       
        if(giaoVien == null) throw new NotFoundIDException();
        else
        {
            giaoVien.Code = request.Code;
            giaoVien.Ten = request.Ten;
            giaoVien.GioiTinh = request.GioiTinh;
            giaoVien.DiaChi = request.DiaChi;
            giaoVien.TruongDangDay = request.TruongDangDay;
            giaoVien.NgaySinh = ngaySinh;
            giaoVien.Email = request.Email;
            giaoVien.SoDienThoai = request.SoDienThoai;
            giaoVien.CoSoId = coSoId;
        }
        if (giaoVien.UserId != null && request.Status.ToLower() == "false")
        {
            await _identityService.disableUser(giaoVien.UserId);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = giaoVien,
            code = 200,
            message = "Cập nhật giáo viên thành công"
        };
    }
}

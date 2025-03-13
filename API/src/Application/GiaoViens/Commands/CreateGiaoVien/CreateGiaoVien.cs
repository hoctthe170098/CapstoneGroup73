using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanViens.Command.CreateNhanVien;
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
public record CreateGiaoVienCommand : IRequest<Output>
{
    public required string Code { get; init; }
    public required string Ten { get; init; }
    public required string GioiTinh { get; init; }
    public required string DiaChi { get; init; }
    public required string TruongDangDay { get; init; }
    public required string NgaySinh { get; init; }
    public required string Email { get; init; }
    public required string SoDienThoai { get; init; }
}
public class CreateGiaoVienCommandHandler : IRequestHandler<CreateGiaoVienCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateGiaoVienCommandHandler(IApplicationDbContext context
        , IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(CreateGiaoVienCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ request header
            var token = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.DiaChi) ||
            string.IsNullOrWhiteSpace(request.TruongDangDay) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.NgaySinh))
        {
            throw new NotFoundDataException("Dữ liệu không được để trống");
        }
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
        Guid coSoId = _identityService.GetCampusId(token);
        if(coSoId == Guid.Empty)
        {
            throw new FormatException("CoSo không hợp lệ");
        }
        // Validate length constraints
        if (request.Code.Length > 20 || request.Ten.Length > 50 || request.DiaChi.Length > 100)
        {
            throw new WrongInputException("Độ dài dữ liệu không hợp lệ!");
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
        if (!string.IsNullOrEmpty(request.Email) && !new EmailAddressAttribute().IsValid(request.Email))
        {
            throw new FormatException("Email không hợp lệ");
        }
        // Check if CoSo exists
        var coSoExists = await _context.CoSos
            .AnyAsync(c => c.Id == coSoId&&c.TrangThai=="open", cancellationToken);
        if (!coSoExists)
        {
            throw new NotFoundDataException("Cơ sở không tồn tại");
        }
        // Check If phone or email exist
        var phoneExists = await _context.GiaoViens.AnyAsync(nv => nv.SoDienThoai == request.SoDienThoai, cancellationToken);
        var emailExists = await _context.GiaoViens.AnyAsync(nv => nv.Email == request.Email, cancellationToken);
        if (phoneExists || emailExists)
        {
            throw new WrongInputException($"Số điện thoại hoặc email đã tồn tại");
        }
        // Validate Code not duplicate
        var exists = await _context.GiaoViens.AnyAsync(gv => gv.Code.Substring(2) == request.Code, cancellationToken);
        if (exists)
        {
            throw new WrongInputException($"Mã giáo viên '{request.Code}' đã tồn tại!");
        }
        // Create identity user     
        var (result, userId) = await _identityService.GenerateUser(request.Ten, request.Code, request.Email);

        if (!result.Succeeded)
        {
            throw new Exception("Tạo tài khoản thất bại: " + string.Join(", ", result.Errors));
        }
        // Create new NhanVien entity
        var giaoVien = new GiaoVien
        {
            Code = "GV"+request.Code,
            Ten = request.Ten,
            GioiTinh = request.GioiTinh,
            DiaChi = request.DiaChi,
            TruongDangDay = request.TruongDangDay,
            NgaySinh = ngaySinh,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            CoSoId = coSoId,
            UserId = userId
        };
        _context.GiaoViens.Add(giaoVien);
        await _context.SaveChangesAsync(cancellationToken);
        // Assign user to role if UserId exists
        if (!string.IsNullOrEmpty(giaoVien.UserId))
        {
            await _identityService.AssignRoleAsync(userId, Roles.Teacher);
        }
        return new Output
        {
            isError = false,
            data = giaoVien,
            code = 200,
            message = "Tạo giáo viên mới thành công"
        };
    }
}




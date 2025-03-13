using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanViens.Command.CreateNhanVien;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
public record CreateGiaoVienCommand : IRequest<Output>
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string TruongDangDay { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public required string Email { get; set; }
    public string? SoDienThoai { get; set; }
    public required Guid CoSoId { get; set; }
}

public class CreateGiaoVienCommandHandler : IRequestHandler<CreateGiaoVienCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public CreateGiaoVienCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Output> Handle(CreateGiaoVienCommand request, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.DiaChi) ||
            string.IsNullOrWhiteSpace(request.TruongDangDay) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            throw new NotFoundDataException("Dữ liệu không được để trống");
        }

        // Validate Code not duplicate
        var exists = await _context.GiaoViens.AnyAsync(gv => gv.Code == request.Code, cancellationToken);
        if (exists)
        {
            throw new WrongInputException($"Mã giáo viên '{request.Code}' đã tồn tại!");
        }

        // Vallidate NgaySinh not in future
        if (request.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new WrongInputException("Ngày sinh không hợp lệ!");
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
        var coSoExists = await _context.CoSos.AnyAsync(c => c.Id == request.CoSoId, cancellationToken);
        if (!coSoExists)
        {
            throw new NotFoundDataException("Cơ sở không tồn tại");
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
            Code = request.Code,
            Ten = request.Ten,
            GioiTinh = request.GioiTinh,
            DiaChi = request.DiaChi,
            TruongDangDay = request.TruongDangDay,
            NgaySinh = request.NgaySinh,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            CoSoId = request.CoSoId,
            UserId = userId
        };

        _context.GiaoViens.Add(giaoVien);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign user to role if UserId exists
        if (!string.IsNullOrEmpty(giaoVien.UserId))
        {
            await _identityService.AssignRoleAsync(userId, "Teacher");
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




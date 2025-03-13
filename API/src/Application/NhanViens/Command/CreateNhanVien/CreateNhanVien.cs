using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.NhanViens.Command.CreateNhanVien;
public record CreateNhanVienCommand : IRequest<Output>
{
    public required string Code { get; init; }
    public required string Ten { get; init; }
    public required string GioiTinh { get; init; }
    public required string DiaChi { get; init; }
    public required DateOnly NgaySinh { get; init; }
    public required string Email { get; init; }
    public string? SoDienThoai { get; init; }
    public required Guid CoSoId { get; init; }
    public required string Role { get; init; }
}

public class CreateNhanVienCommandHandler : IRequestHandler<CreateNhanVienCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService; 

    public CreateNhanVienCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Output> Handle(CreateNhanVienCommand request, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.DiaChi) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            throw new NotFoundDataException("Dữ liệu không được để trống");
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
        var nhanVien = new NhanVien
        {
            Code = request.Code,
            Ten = request.Ten,
            GioiTinh = request.GioiTinh,
            DiaChi = request.DiaChi,
            NgaySinh = request.NgaySinh,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            CoSoId = request.CoSoId,
            UserId = userId
        };

        _context.NhanViens.Add(nhanVien);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign user to role if UserId exists
        if (!string.IsNullOrEmpty(nhanVien.UserId))
        {
            await _identityService.AssignRoleAsync(userId, request.Role);
        }

        return new Output
        {
            isError = false,
            data = nhanVien,
            code = 200,
            message = "Tạo nhân viên mới thành công"
        };
    }
   
}



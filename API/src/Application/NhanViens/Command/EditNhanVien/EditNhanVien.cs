using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;
namespace StudyFlow.Application.NhanViens.Command.EditNhanVien;
public class EditNhanVienCommand : IRequest<Output>
{
    public required string Code { get; init; }
    public required string Ten { get; init; }
    public required string GioiTinh { get; init; }
    public required string DiaChi { get; init; }
    public required string NgaySinh { get; init; }
    public required string Email { get; init; }
    public required string SoDienThoai { get; init; }
    public required string CoSoId { get; init; }
    public required string Role { get; init; }
    public required string Status { get; init; }
}

public class EditNhanVienCommandHandler : IRequestHandler<EditNhanVienCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService; 

    public EditNhanVienCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<Output> Handle(EditNhanVienCommand request, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.DiaChi)||
            string.IsNullOrWhiteSpace(request.CoSoId)
            )
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
            throw new WrongInputException("Nhân viên phải đủ 18 tuổi trở lên");
        }
        //Validate Status
        if (request.Status.ToLower() != "true" && request.Status.ToLower() != "false")
            throw new FormatException("Trạng thái không hợp lệ" +
                ", chỉ có thể là true hoặc false");
        // Validate role
        if (request.Role != Roles.CampusManager && request.Role != Roles.LearningManager)
            throw new WrongInputException("Vai trò không hợp lệ!");
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
        // Validate CoSoId (Campus ID) format
        if (!Guid.TryParse(request.CoSoId, out Guid coSoId))
        {
            throw new FormatException("CoSoId không hợp lệ. Định dạng phải là Guid");
        }
        // Check if CoSo exists
        var coSoExists = await _context.CoSos
            .AnyAsync(c => c.Id == coSoId, cancellationToken);
        if (!coSoExists)
        {
            throw new NotFoundDataException("Cơ sở không tồn tại");
        }

        var nhanVien = await _context.NhanViens.FindAsync(new object[] { request.Code }, cancellationToken);

        if (nhanVien == null) throw new NotFoundIDException();
        else
        {
            if(nhanVien.UserId==null) throw new NotFoundIDException();
            nhanVien.Ten = request.Ten;
            nhanVien.GioiTinh = request.GioiTinh;
            nhanVien.DiaChi = request.DiaChi;
            nhanVien.NgaySinh = ngaySinh;
            nhanVien.Email = request.Email;
            nhanVien.SoDienThoai = request.SoDienThoai;
            nhanVien.CoSoId = coSoId;
            await _identityService.AssignRoleAsync(nhanVien.UserId, request.Role);
        }
        if (nhanVien.UserId != null)
        {
            await _identityService.changeEmail(nhanVien.UserId,request.Email);
        }
        if (nhanVien.UserId != null && request.Status.ToLower() == "false")
        {
            await _identityService.disableUser(nhanVien.UserId);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = nhanVien,
            code = 200,
            message = "Cập nhật nhân viên thành công"
        };
    }
}

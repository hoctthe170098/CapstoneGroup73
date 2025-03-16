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
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.HocSinhs.Commands.CreateHocSinh;
public record CreateHocSinhCommand : IRequest<Output>
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string Lop { get; set; }
    public required string TruongDangHoc { get; set; }
    public required string NgaySinh { get; set; }
    public required string Email { get; set; }
    public required string SoDienThoai { get; set; }
    public string? ChinhSachId { get; set; }
}

public class CreateHocSinhCommandHandler : IRequestHandler<CreateHocSinhCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateHocSinhCommandHandler(IApplicationDbContext context
        , IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(CreateHocSinhCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ request header
        var token = _httpContextAccessor.HttpContext?
        .Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.DiaChi) ||
            string.IsNullOrWhiteSpace(request.TruongDangHoc) ||
            string.IsNullOrWhiteSpace(request.Lop) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai)||
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
        var fiveYearsAgo = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        if (ngaySinh > fiveYearsAgo)
        {
            throw new WrongInputException("Học sinh phải đủ 5 tuổi trở lên");
        }
        Guid coSoId = _identityService.GetCampusId(token);
        if (coSoId == Guid.Empty)
        {
            throw new FormatException("CoSo không hợp lệ");
        }
        // Validate length constraints
        if (request.Code.Length > 20 || request.Ten.Length > 50 || request.DiaChi.Length > 200
            ||request.Lop.Length>20)
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
        var coSoExists = await _context.CoSos.AnyAsync(c => c.Id == coSoId, cancellationToken);
        if (!coSoExists)
        {
            throw new NotFoundDataException("Cơ sở không tồn tại");
        }
        int chinhsach_id;
        if (request.ChinhSachId == null || request.ChinhSachId.Trim() == "")
        {
            chinhsach_id = 0;
        }
        else
        {
            var check = Int32.TryParse(request.ChinhSachId,out chinhsach_id);
            if (!check)
            {
                throw new FormatException("Chính sách không hợp lệ");
            }
            else
            {
                var checkExist = await _context.ChinhSaches
                    .AnyAsync(c => c.Id == chinhsach_id);
                if (!checkExist) throw new Exception("Chính sách này không tồn tại!");
            }
        }
        // Validate Code duplicate
        var exists = await _context.HocSinhs
            .AnyAsync(gv => gv.Code.Substring(2) == request.Code, cancellationToken);
        if (exists)
        {
            throw new WrongInputException($"Mã học viên '{request.Code}' đã tồn tại!");
        }
        // Create identity user     
        var (result, userId) = await _identityService.GenerateUser(request.Ten, request.Code, request.Email);
        if (!result.Succeeded)
        {
            throw new Exception("Tạo tài khoản thất bại: " + string.Join(", ", result.Errors));
        }
        // Create new NhanVien entity
        var hocSinh = new HocSinh
        {
            Code = request.Code,
            Ten = request.Ten,
            GioiTinh = request.GioiTinh,
            DiaChi = request.DiaChi,
            TruongDangHoc = request.TruongDangHoc,
            NgaySinh = ngaySinh,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            ChinhSachId = (chinhsach_id>0)?chinhsach_id:null,
            Lop = request.Lop,
            CoSoId = coSoId,
            UserId = userId
        };
        _context.HocSinhs.Add(hocSinh);
        await _context.SaveChangesAsync(cancellationToken);
        // Assign user to role if UserId exists
        if (!string.IsNullOrEmpty(hocSinh.UserId))
        {
            await _identityService.AssignRoleAsync(userId, Roles.Student);
        }
        return new Output
        {
            isError = false,
            data = hocSinh,
            code = 200,
            message = "Tạo học viên mới thành công"
        };
    }
}

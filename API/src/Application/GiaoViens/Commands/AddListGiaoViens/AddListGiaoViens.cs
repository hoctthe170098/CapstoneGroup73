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
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
public class AddListGiaoViensCommand : IRequest<Output>
{
    public required List<CreateGiaoVienCommand> GiaoViens { get; set; }
}
public class AddListGiaoViensCommandHandler : IRequestHandler<AddListGiaoViensCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AddListGiaoViensCommandHandler(IApplicationDbContext context
        , IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(AddListGiaoViensCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ request header
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        if (request.GiaoViens == null || !request.GiaoViens.Any())
        {
            throw new WrongInputException("Danh sách giáo viên không được rỗng.");
        }
        var codes = request.GiaoViens.Select(gv => gv.Code).ToList();
        // Validate existing code
        var existingCodes = await _context.GiaoViens.Where(gv => codes.Contains(gv.Code)).Select(gv => gv.Code).ToListAsync(cancellationToken);

        if (existingCodes.Any())
        {
            throw new WrongInputException($"Mã giáo viên đã tồn tại: {string.Join(", ", existingCodes)}");
        }
        var giaoViensToAdd = new List<GiaoVien>();
        var errors = new List<string>(); // Danh sách lỗi
        foreach (var req in request.GiaoViens)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(req.Code) ||
                    string.IsNullOrWhiteSpace(req.Ten) ||
                    string.IsNullOrWhiteSpace(req.GioiTinh) ||
                    string.IsNullOrWhiteSpace(req.DiaChi) ||
                    string.IsNullOrWhiteSpace(req.SoDienThoai) ||
                    string.IsNullOrWhiteSpace(req.TruongDangDay)||
                    string.IsNullOrWhiteSpace(req.SoDienThoai) ||
                    string.IsNullOrWhiteSpace(req.Email) ||
                    string.IsNullOrWhiteSpace(req.NgaySinh))
                {
                    throw new NotFoundDataException($"Dữ liệu không hợp lệ cho giáo viên có mã {req.Code}");
                }
                // Validate NgaySinh (Date of Birth) format
                if (!DateOnly.TryParseExact(req.NgaySinh, "yyyy-MM-dd", out DateOnly ngaySinh))
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
                if (coSoId == Guid.Empty)
                {
                    throw new FormatException("CoSo không hợp lệ");
                }
                if (req.Code.Length > 20 || req.Ten.Length > 50 || req.DiaChi.Length > 100)
                {
                    throw new WrongInputException($"Dữ liệu vượt quá giới hạn cho giáo viên có mã {req.Code}");
                }

                if (!string.IsNullOrEmpty(req.SoDienThoai) &&
                    (req.SoDienThoai.Length > 11 || !req.SoDienThoai.StartsWith("0") || !req.SoDienThoai.All(char.IsDigit)))
                {
                    throw new FormatException($"Số điện thoại không hợp lệ cho giáo viên có mã {req.Code}");
                }
                if (!string.IsNullOrEmpty(req.Email) && !new EmailAddressAttribute().IsValid(req.Email))
                {
                    throw new FormatException($"Email không hợp lệ cho giáo viên có mã {req.Code}");
                }
                var coSoExists = await _context.CoSos.AnyAsync(c => c.Id == coSoId && c.TrangThai == "open", cancellationToken);
                if (!coSoExists)
                {
                    throw new NotFoundDataException($"Cơ sở không tồn tại cho giáo viên có mã {req.Code}");
                }
                // Validate Code not duplicate
                var exists = await _context.GiaoViens.AnyAsync(gv => gv.Code.Substring(2) == req.Code, cancellationToken);
                if (exists)
                {
                    throw new WrongInputException($"Mã giáo viên '{req.Code}' đã tồn tại!");
                }
                // Check If phone or email exist
                var phoneExists = await _context.GiaoViens.AnyAsync(nv => nv.SoDienThoai == req.SoDienThoai, cancellationToken);
                var emailExists = await _context.GiaoViens.AnyAsync(nv => nv.Email == req.Email, cancellationToken);
                if (phoneExists || emailExists)
                {
                    throw new WrongInputException($"Số điện thoại hoặc email đã tồn tại");
                }
                // Create identity user
                var (result, userId) = await _identityService.GenerateUser(req.Ten, req.Code, req.Email);
                if (!result.Succeeded)
                {
                    throw new Exception($"Tạo tài khoản thất bại cho giáo viên có mã {req.Code}: {string.Join(", ", result.Errors)}");
                }

                giaoViensToAdd.Add(new GiaoVien
                {
                    Code = "GV"+req.Code,
                    Ten = req.Ten,
                    GioiTinh = req.GioiTinh,
                    DiaChi = req.DiaChi,
                    TruongDangDay = req.TruongDangDay,
                    NgaySinh = ngaySinh,
                    Email = req.Email,
                    SoDienThoai = req.SoDienThoai,
                    CoSoId = coSoId,
                    UserId = userId
                });
            }
            catch (NotFoundDataException ex)
            {
                errors.Add($"Lỗi cho giáo viên {req.Code}: {ex.Message}");
            }
            catch (FormatException ex)
            {
                errors.Add($"Lỗi định dạng cho giáo viên {req.Code}: {ex.Message}");
            }
            catch (WrongInputException ex)
            {
                errors.Add($"Lỗi dữ liệu cho giáo viên {req.Code}: {ex.Message}");
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi không xác định cho giáo viên {req.Code}: {ex.Message}");
            }
        }
        // Thêm giáo viên thành công (nếu có)
        if (giaoViensToAdd.Any())
        {
            await _context.GiaoViens.AddRangeAsync(giaoViensToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var giaoVien in giaoViensToAdd)
            {
                await _identityService.AssignRoleAsync(giaoVien.UserId!, Roles.Teacher);
            }
        }
        // Trả về kết quả
        return new Output
        {
            isError = errors.Any(),
            data = giaoViensToAdd,
            code = errors.Any() ? 500 : 200, // Mã lỗi nếu có lỗi
            message = errors.Any() ? $"Có lỗi xảy ra: {string.Join("; ", errors)}" : $"Tạo {giaoViensToAdd.Count} giáo viên thành công."
        };
    }
}

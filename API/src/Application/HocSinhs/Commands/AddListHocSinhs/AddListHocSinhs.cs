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
using StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
using StudyFlow.Application.HocSinhs.Commands.CreateHocSinh;
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.HocSinhs.Commands.AddListHocSinhs;
public record AddListHocSinhsCommand : IRequest<Output>
{
    public required List<CreateHocSinhCommand> HocSinhs { get; set; }
}

public class AddListHocSinhsCommandHandler : IRequestHandler<AddListHocSinhsCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AddListHocSinhsCommandHandler(IApplicationDbContext context, IIdentityService identityService
        , IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(AddListHocSinhsCommand request, CancellationToken cancellationToken)
    {
        // Lấy token từ request header
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        Guid coSoId = _identityService.GetCampusId(token);
        if (coSoId == Guid.Empty)
        {
            throw new FormatException("CoSo không hợp lệ");
        }
        if (request.HocSinhs == null || !request.HocSinhs.Any())
        {
            throw new WrongInputException("Danh sách học viên không được rỗng.");
        }
        var codes = request.HocSinhs.Select(hs => hs.Code).ToList();
        // Validate existing code
        var existingCodes = await _context.HocSinhs
            .Where(gv => codes.Contains(gv.Code))
            .Select(gv => gv.Code)
            .ToListAsync(cancellationToken);
        if (existingCodes.Any())
        {
            throw new WrongInputException($"Mã học viên đã tồn tại: {string.Join(", ", existingCodes)}");
        }
        var hocSinhsToAdd = new List<HocSinh>();
        var errors = new List<string>();
        foreach (var req in request.HocSinhs)
        {
                if (string.IsNullOrWhiteSpace(req.Code) ||
             string.IsNullOrWhiteSpace(req.Ten) ||
             string.IsNullOrWhiteSpace(req.GioiTinh) ||
             string.IsNullOrWhiteSpace(req.DiaChi) ||
             string.IsNullOrWhiteSpace(req.TruongDangHoc) ||
             string.IsNullOrWhiteSpace(req.Lop) ||
             string.IsNullOrWhiteSpace(req.Email) ||
             string.IsNullOrWhiteSpace(req.SoDienThoai) ||
             string.IsNullOrWhiteSpace(req.NgaySinh))
                {
                    throw new NotFoundDataException($"Dữ liệu học viên {req.Code} không được để trống");
                }
                // Validate NgaySinh (Date of Birth) format
                if (!DateOnly.TryParseExact(req.NgaySinh, "yyyy-MM-dd", out DateOnly ngaySinh))
                {
                    throw new FormatException($"Ngày sinh học viên {req.Code} không hợp lệ. Định dạng phải là yyyy-MM-dd");
                }
                // Validate NgaySinh (Date of Birth) is at least 5 years old
                var fiveYearsAgo = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
                if (ngaySinh > fiveYearsAgo)
                {
                    throw new WrongInputException($"Học sinh {req.Code} phải đủ 5 tuổi trở lên");
                }
                // Validate length constraints
                if (req.Code.Length > 20 || req.Ten.Length > 50 || req.DiaChi.Length > 200
                    || req.Lop.Length > 20)
                {
                    throw new WrongInputException($"Độ dài dữ liệu Học sinh {req.Code} không hợp lệ!");
                }
                // Validate phone number 
                if (!string.IsNullOrEmpty(req.SoDienThoai))
                {
                    if (req.SoDienThoai.Length > 11 || !req.SoDienThoai.StartsWith("0") || !req.SoDienThoai.All(char.IsDigit))
                    {
                        throw new FormatException($"Số điện thoại Học sinh {req.Code} không hợp lệ");
                    }
                }
                // Validate email 
                if (!string.IsNullOrEmpty(req.Email) && !new EmailAddressAttribute().IsValid(req.Email))
                {
                    throw new FormatException($"Email Học sinh {req.Code} không hợp lệ");
                }
                // Check if CoSo exists
                var coSoExists = await _context.CoSos.AnyAsync(c => c.Id == coSoId, cancellationToken);
                if (!coSoExists)
                {
                    throw new NotFoundDataException($"Cơ sở Học sinh {req.Code} không tồn tại");
                }
                int chinhsach_id;
                if (req.ChinhSachId == null || req.ChinhSachId.Trim() == "")
                {
                    chinhsach_id = 0;
                }
                else
                {
                    var check = Int32.TryParse(req.ChinhSachId, out chinhsach_id);
                    if (!check)
                    {
                        throw new FormatException($"Chính sách Học sinh {req.Code} không hợp lệ");
                    }
                    else
                    {
                        var checkExist = await _context.ChinhSaches
                            .AnyAsync(c => c.Id == chinhsach_id);
                        if (!checkExist) throw new NotFoundDataException($"Chính sách Học sinh {req.Code} không tồn tại!");
                    }
                }
        }
        try
        {
            foreach (var req in request.HocSinhs)
            {
                // Create identity user
                var (result, userId) = await _identityService.GenerateUser(req.Ten, req.Code, req.Email);
                if (!result.Succeeded)
                {
                    throw new Exception($"Tạo tài khoản thất bại cho học viên có mã {req.Code}: {string.Join(", ", result.Errors)}");
                }
                else
                {
                    hocSinhsToAdd.Add(new HocSinh
                    {
                        Code = req.Code,
                        Ten = req.Ten,
                        GioiTinh = req.GioiTinh,
                        DiaChi = req.DiaChi,
                        TruongDangHoc = req.TruongDangHoc,
                        Lop = req.Lop,
                        NgaySinh = DateOnly.Parse(req.NgaySinh),
                        Email = req.Email,
                        SoDienThoai = req.SoDienThoai,
                        CoSoId = coSoId,
                        ChinhSachId = (req.ChinhSachId == null || req.ChinhSachId.Trim() == "") ? null : Int32.Parse(req.ChinhSachId),
                        UserId = userId
                    });
                }
            }
        }catch(Exception ex)
        {
            errors.Add(ex.Message);
        }
        if (hocSinhsToAdd.Any())
        {
            await _context.HocSinhs.AddRangeAsync(hocSinhsToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            foreach (var hocSinh in hocSinhsToAdd)
            {
                await _identityService.AssignRoleAsync(hocSinh.UserId!, Roles.Student);
            }
        }
        // Trả về kết quả
        return new Output
        {
            isError = errors.Any(),
            data = hocSinhsToAdd,
            code = errors.Any() ? 500 : 200, // Mã lỗi nếu có lỗi
            message = errors.Any() ? $"Có lỗi xảy ra: {string.Join("; ", errors)}" : $"Tạo {hocSinhsToAdd.Count} học sinh thành công."
        };
    }
}

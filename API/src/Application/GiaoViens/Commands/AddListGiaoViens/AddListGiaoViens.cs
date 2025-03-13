using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
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

    public AddListGiaoViensCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Output> Handle(AddListGiaoViensCommand request, CancellationToken cancellationToken)
    {
        if (request.GiaoViens == null || !request.GiaoViens.Any())
        {
            throw new WrongInputException("Danh sách giáo viên không được rỗng.");
        }

        var codes = request.GiaoViens.Select(gv => gv.Code).ToList();

        // Validate existing code
        var existingCodes = await _context.GiaoViens
            .Where(gv => codes.Contains(gv.Code))
            .Select(gv => gv.Code)
            .ToListAsync(cancellationToken);

        if (existingCodes.Any())
        {
            throw new WrongInputException($"Mã giáo viên đã tồn tại: {string.Join(", ", existingCodes)}");
        }

        var giaoViensToAdd = new List<GiaoVien>();

        foreach (var req in request.GiaoViens)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(req.Code) ||
                string.IsNullOrWhiteSpace(req.Ten) ||
                string.IsNullOrWhiteSpace(req.GioiTinh) ||
                string.IsNullOrWhiteSpace(req.DiaChi) ||
                string.IsNullOrWhiteSpace(req.SoDienThoai) ||
                string.IsNullOrWhiteSpace(req.TruongDangDay))
            {
                throw new NotFoundDataException($"Dữ liệu không hợp lệ cho giáo viên có mã {req.Code}");
            }

            if (req.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new WrongInputException($"Ngày sinh không hợp lệ cho giáo viên có mã {req.Code}");
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

            var coSoExists = await _context.CoSos.AnyAsync(c => c.Id == req.CoSoId, cancellationToken);
            if (!coSoExists)
            {
                throw new NotFoundDataException($"Cơ sở không tồn tại cho giáo viên có mã {req.Code}");
            }

            // Create identity user
            var (result, userId) = await _identityService.GenerateUser(req.Ten, req.Code, req.Email);
            if (!result.Succeeded)
            {
                throw new Exception($"Tạo tài khoản thất bại cho giáo viên có mã {req.Code}: {string.Join(", ", result.Errors)}");
            }

            giaoViensToAdd.Add(new GiaoVien
            {
                Code = req.Code,
                Ten = req.Ten,
                GioiTinh = req.GioiTinh,
                DiaChi = req.DiaChi,
                TruongDangDay = req.TruongDangDay,
                NgaySinh = req.NgaySinh,
                Email = req.Email,
                SoDienThoai = req.SoDienThoai,
                CoSoId = req.CoSoId,
                UserId = userId
            });
        }

        await _context.GiaoViens.AddRangeAsync(giaoViensToAdd, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var giaoVien in giaoViensToAdd)
        {
            await _identityService.AssignRoleAsync(giaoVien.UserId!, "Teacher");
        }

        return new Output
        {
            isError = false,
            data = giaoViensToAdd,
            code = 200,
            message = $"Tạo {giaoViensToAdd.Count} giáo viên thành công."
        };
    }
}

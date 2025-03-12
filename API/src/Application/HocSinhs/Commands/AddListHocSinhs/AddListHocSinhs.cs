using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
using StudyFlow.Application.HocSinhs.Commands.CreateHocSinh;
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

    public AddListHocSinhsCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Output> Handle(AddListHocSinhsCommand request, CancellationToken cancellationToken)
    {
        if (request.HocSinhs == null || !request.HocSinhs.Any())
        {
            throw new WrongInputException("Danh sách giáo viên không được rỗng.");
        }

        var codes = request.HocSinhs.Select(gv => gv.Code).ToList();

        // Validate existing code
        var existingCodes = await _context.HocSinhs
            .Where(gv => codes.Contains(gv.Code))
            .Select(gv => gv.Code)
            .ToListAsync(cancellationToken);

        if (existingCodes.Any())
        {
            throw new WrongInputException($"Mã giáo viên đã tồn tại: {string.Join(", ", existingCodes)}");
        }

        var hocSinhsToAdd = new List<HocSinh>();

        foreach (var req in request.HocSinhs)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(req.Code) ||
                string.IsNullOrWhiteSpace(req.Ten) ||
                string.IsNullOrWhiteSpace(req.GioiTinh) ||
                string.IsNullOrWhiteSpace(req.DiaChi) ||
                string.IsNullOrWhiteSpace(req.TruongDangHoc) ||
                string.IsNullOrWhiteSpace(req.Lop) ||
                string.IsNullOrWhiteSpace(req.Email))
            {
                throw new NotFoundDataException($"Dữ liệu không hợp lệ cho học viên có mã {req.Code}");
            }

            if (req.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new WrongInputException($"Ngày sinh không hợp lệ cho học viên có mã {req.Code}");
            }

            if (req.Code.Length > 20 || req.Ten.Length > 50 || req.DiaChi.Length > 100)
            {
                throw new WrongInputException($"Dữ liệu vượt quá giới hạn cho học viên có mã {req.Code}");
            }

            if (!string.IsNullOrEmpty(req.SoDienThoai) &&
                (req.SoDienThoai.Length > 11 || !req.SoDienThoai.StartsWith("0") || !req.SoDienThoai.All(char.IsDigit)))
            {
                throw new FormatException($"Số điện thoại không hợp lệ cho học viên có mã {req.Code}");
            }

            if (!string.IsNullOrEmpty(req.Email) && !new EmailAddressAttribute().IsValid(req.Email))
            {
                throw new FormatException($"Email không hợp lệ cho học viên có mã {req.Code}");
            }

            var coSoExists = await _context.CoSos.AnyAsync(c => c.Id == req.CoSoId, cancellationToken);
            if (!coSoExists)
            {
                throw new NotFoundDataException($"Cơ sở không tồn tại cho học viên có mã {req.Code}");
            }

            // Create identity user
            var (result, userId) = await _identityService.GenerateUser(req.Ten, req.Code);
            if (!result.Succeeded)
            {
                throw new Exception($"Tạo tài khoản thất bại cho học viên có mã {req.Code}: {string.Join(", ", result.Errors)}");
            }

            hocSinhsToAdd.Add(new HocSinh
            {
                Code = req.Code,
                Ten = req.Ten,
                GioiTinh = req.GioiTinh,
                DiaChi = req.DiaChi,
                TruongDangHoc = req.TruongDangHoc,
                Lop = req.Lop,
                NgaySinh = req.NgaySinh,
                Email = req.Email,
                SoDienThoai = req.SoDienThoai,
                CoSoId = req.CoSoId,
                ChinhSachId = req.ChinhSachId,
                UserId = userId
            });
        }

        await _context.HocSinhs.AddRangeAsync(hocSinhsToAdd, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var giaoVien in hocSinhsToAdd)
        {
            await _identityService.AssignRoleAsync(giaoVien.UserId!, "Teacher");
        }

        return new Output
        {
            isError = false,
            data = hocSinhsToAdd,
            code = 200,
            message = $"Tạo {hocSinhsToAdd.Count} hóc viên thành công."
        };
    }
}

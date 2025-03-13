using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;

namespace StudyFlow.Application.HocSinhs.Commands.EditHocSinh;
public record EditHocSinhCommand : IRequest<Output>
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string Lop { get; set; }
    public required string TruongDangHoc { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public required string Email { get; set; }
    public required string SoDienThoai { get; set; }
    public int? ChinhSachId { get; set; }
    public required Guid CoSoId { get; set; }
}

public class EditHocSinhCommandHandler : IRequestHandler<EditHocSinhCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public EditHocSinhCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<Output> Handle(EditHocSinhCommand request, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.Ten) ||
            string.IsNullOrWhiteSpace(request.GioiTinh) ||
            string.IsNullOrWhiteSpace(request.DiaChi) ||
            string.IsNullOrWhiteSpace(request.TruongDangHoc) ||
            string.IsNullOrWhiteSpace(request.Lop) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            throw new NotFoundDataException("Dữ liệu không được để trống");
        }

        // Validate Code not duplicate
        var exists = await _context.HocSinhs.AnyAsync(hs => hs.Code == request.Code && hs.Code != request.Code , cancellationToken);
        if (exists)
        {
            throw new WrongInputException($"Mã học viên '{request.Code}' đã tồn tại!");
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

        var hocSinh = await _context.HocSinhs.FindAsync(new object[] { request.Code }, cancellationToken);

        if (hocSinh == null) throw new NotFoundIDException();
        else
        {

            hocSinh.Code = request.Code;
            hocSinh.Ten = request.Ten;
            hocSinh.GioiTinh = request.GioiTinh;
            hocSinh.DiaChi = request.DiaChi;
            hocSinh.TruongDangHoc = request.TruongDangHoc;
            hocSinh.Lop = request.Lop;
            hocSinh.NgaySinh = request.NgaySinh;
            hocSinh.Email = request.Email;
            hocSinh.SoDienThoai = request.SoDienThoai;
            hocSinh.CoSoId = request.CoSoId;
            hocSinh.ChinhSachId = request.ChinhSachId;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = hocSinh,
            code = 200,
            message = "Cập nhật học viên thành công"
        };
    }
}



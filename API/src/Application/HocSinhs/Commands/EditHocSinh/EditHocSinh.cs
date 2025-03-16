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
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.HocSinhs.Commands.EditHocSinh;
public record EditHocSinhCommand : IRequest<Output>
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
    public required string Status { get; init; }
}

public class EditHocSinhCommandHandler : IRequestHandler<EditHocSinhCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public EditHocSinhCommandHandler(IApplicationDbContext context
        , IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(EditHocSinhCommand request, CancellationToken cancellationToken)
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
            string.IsNullOrWhiteSpace(request.TruongDangHoc) ||
            string.IsNullOrWhiteSpace(request.Lop) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.Email)||
            string.IsNullOrWhiteSpace(request.NgaySinh)||
            string.IsNullOrWhiteSpace(request.Status))
        {
            throw new NotFoundDataException("Dữ liệu không được để trống");
        }
        //Validate Status
        if (request.Status.ToLower() != "true" && request.Status.ToLower() != "false")
            throw new FormatException("Trạng thái không hợp lệ" +
                ", chỉ có thể là true hoặc false");
        Guid coSoId = _identityService.GetCampusId(token);
        if (coSoId == Guid.Empty)
        {
            throw new FormatException("CoSo không hợp lệ");
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
        // Validate length constraints
        if (request.Code.Length > 20 || request.Ten.Length > 50 || request.DiaChi.Length > 200)
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
            var check = Int32.TryParse(request.ChinhSachId, out chinhsach_id);
            if (!check)
            {
                throw new FormatException();
            }
            else
            {
                var checkExist = await _context.ChinhSaches
                    .AnyAsync(c => c.Id == chinhsach_id);
                if (!checkExist) throw new Exception("Chính sách này không tồn tại!");
            }
        }
        var hocSinh = await _context.HocSinhs
            .FirstOrDefaultAsync(hs => hs.Code == request.Code
        && hs.CoSoId == coSoId, cancellationToken);
        if (hocSinh == null) throw new NotFoundIDException();
        else
        {
            hocSinh.Ten = request.Ten;
            hocSinh.GioiTinh = request.GioiTinh;
            hocSinh.DiaChi = request.DiaChi;
            hocSinh.TruongDangHoc = request.TruongDangHoc;
            hocSinh.Lop = request.Lop;
            hocSinh.NgaySinh = ngaySinh;
            hocSinh.Email = request.Email;
            hocSinh.SoDienThoai = request.SoDienThoai;
            hocSinh.ChinhSachId = (chinhsach_id > 0) ? chinhsach_id : null;
        }
        if (hocSinh.UserId != null)
        {
            await _identityService
                .UpdateStatusUser(hocSinh.UserId, Boolean.Parse(request.Status));
        }
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = hocSinh,
            code = 200,
            message = "Cập nhật học sinh thành công"
        };
    }
}



using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public record EditLichHocCommand : IRequest<Output>
{
    public Guid Id { get; set; }
    public int? Thu { get; set; }
    public int? PhongId { get; set; }
    public string? TenLop { get; set; }
    public string? GioBatDau { get; set; }
    public string? GioKetThuc { get; set; }
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public int? HocPhi { get; set; }
    public string? TrangThai { get; set; }
    public string? GiaoVienCode { get; set; }
    public int? ChuongTrinhId { get; set; }
}

public class EditLichHocCommandHandler : IRequestHandler<EditLichHocCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public EditLichHocCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(EditLichHocCommand request, CancellationToken cancellationToken)
    {
        var lichHoc = await _context.LichHocs.FindAsync(request.Id)
            ?? throw new NotFoundDataException($"Không tìm thấy lịch học với ID {request.Id}.");

        if (!request.Thu.HasValue && !request.PhongId.HasValue && string.IsNullOrWhiteSpace(request.TenLop) &&
            string.IsNullOrWhiteSpace(request.GioBatDau) && string.IsNullOrWhiteSpace(request.GioKetThuc) && !request.HocPhi.HasValue &&
            string.IsNullOrWhiteSpace(request.TrangThai) && string.IsNullOrWhiteSpace(request.GiaoVienCode) &&
            !request.ChuongTrinhId.HasValue)
        {
            throw new Exception("Ít nhất một trường cần được cập nhật.");
        }

        if (request.PhongId.HasValue)
        {
            var phong = await _context.Phongs.FindAsync(request.PhongId.Value);
            if (phong == null)
            {
                throw new NotFoundDataException("Phòng không tồn tại.");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.GiaoVienCode))
        {
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == request.GiaoVienCode, cancellationToken);
            if (giaoVien == null)
            {
                throw new NotFoundDataException("Giáo viên không tồn tại.");
            }
        }

        if (request.ChuongTrinhId.HasValue)
        {
            var chuongTrinh = await _context.ChuongTrinhs.FindAsync(request.ChuongTrinhId.Value);
            if (chuongTrinh == null)
            {
                throw new NotFoundDataException("Chương trình không tồn tại.");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.GioBatDau) && !string.IsNullOrWhiteSpace(request.GioKetThuc))
        {
            if (!TimeOnly.TryParse(request.GioBatDau, out var gioBatDau) ||
                !TimeOnly.TryParse(request.GioKetThuc, out var gioKetThuc))
            {
                throw new Exception("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");
            }
            if (gioBatDau >= gioKetThuc)
            {
                throw new Exception("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");
            }
            lichHoc.GioBatDau = gioBatDau;
            lichHoc.GioKetThuc = gioKetThuc;
        }

        if (request.Thu.HasValue) lichHoc.Thu = request.Thu.Value;
        if (request.PhongId.HasValue) lichHoc.PhongId = request.PhongId.Value;
        if (!string.IsNullOrWhiteSpace(request.TenLop)) lichHoc.TenLop = request.TenLop;
        if (request.HocPhi.HasValue) lichHoc.HocPhi = request.HocPhi.Value;
        if (!string.IsNullOrWhiteSpace(request.TrangThai)) lichHoc.TrangThai = request.TrangThai;
        if (!string.IsNullOrWhiteSpace(request.GiaoVienCode)) lichHoc.GiaoVienCode = request.GiaoVienCode;
        if (request.ChuongTrinhId.HasValue) lichHoc.ChuongTrinhId = request.ChuongTrinhId.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = lichHoc,
            code = 200,
            message = "Cập nhật lịch học thành công."
        };
    }
}

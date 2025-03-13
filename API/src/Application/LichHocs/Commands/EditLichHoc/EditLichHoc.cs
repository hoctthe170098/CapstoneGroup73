using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public record EditLichHocCommand : IRequest<Output>
{
   public required EditlLichHocDto LichHocDto { get; init; }
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
        var lichHocDto = request.LichHocDto;

        var lichHoc = await _context.LichHocs.FindAsync(lichHocDto.Id)
            ?? throw new NotFoundDataException($"Không tìm thấy lịch học với ID {lichHocDto.Id}.");

        if (!lichHocDto.Thu.HasValue && !lichHocDto.PhongId.HasValue && string.IsNullOrWhiteSpace(lichHocDto.TenLop) &&
            string.IsNullOrWhiteSpace(lichHocDto.GioBatDau) && string.IsNullOrWhiteSpace(lichHocDto.GioKetThuc) && !lichHocDto.HocPhi.HasValue &&
            string.IsNullOrWhiteSpace(lichHocDto.TrangThai) && string.IsNullOrWhiteSpace(lichHocDto.GiaoVienCode) &&
            !lichHocDto.ChuongTrinhId.HasValue)
        {
            throw new Exception("Ít nhất một trường cần được cập nhật.");
        }

        if (lichHocDto.PhongId.HasValue)
        {
            var phong = await _context.Phongs.FindAsync(lichHocDto.PhongId.Value);
            if (phong == null)
                throw new NotFoundDataException("Phòng không tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(lichHocDto.GiaoVienCode))
        {
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == lichHocDto.GiaoVienCode, cancellationToken);
            if (giaoVien == null)
                throw new NotFoundDataException("Giáo viên không tồn tại.");
        }

        if (lichHocDto.ChuongTrinhId.HasValue)
        {
            var chuongTrinh = await _context.ChuongTrinhs.FindAsync(lichHocDto.ChuongTrinhId.Value);
            if (chuongTrinh == null)
                throw new NotFoundDataException("Chương trình không tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(lichHocDto.GioBatDau) && !string.IsNullOrWhiteSpace(lichHocDto.GioKetThuc))
        {
            var gioBatDau = TimeOnly.Parse(lichHocDto.GioBatDau);
            var gioKetThuc = TimeOnly.Parse(lichHocDto.GioKetThuc);

            if (gioBatDau >= gioKetThuc)
                throw new Exception("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

            lichHoc.GioBatDau = gioBatDau;
            lichHoc.GioKetThuc = gioKetThuc;
        }

        if (lichHocDto.Thu.HasValue) lichHoc.Thu = lichHocDto.Thu.Value;
        if (lichHocDto.PhongId.HasValue) lichHoc.PhongId = lichHocDto.PhongId.Value;
        if (!string.IsNullOrWhiteSpace(lichHocDto.TenLop)) lichHoc.TenLop = lichHocDto.TenLop;
        if (lichHocDto.HocPhi.HasValue) lichHoc.HocPhi = lichHocDto.HocPhi.Value;
        if (!string.IsNullOrWhiteSpace(lichHocDto.TrangThai)) lichHoc.TrangThai = lichHocDto.TrangThai;
        if (!string.IsNullOrWhiteSpace(lichHocDto.GiaoVienCode)) lichHoc.GiaoVienCode = lichHocDto.GiaoVienCode;
        if (lichHocDto.ChuongTrinhId.HasValue) lichHoc.ChuongTrinhId = lichHocDto.ChuongTrinhId.Value;

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

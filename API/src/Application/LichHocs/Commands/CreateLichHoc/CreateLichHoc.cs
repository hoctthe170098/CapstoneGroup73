using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
public record CreateLichHocCommand : IRequest<Output>
{
    public required CreateLichHocDto LichHocDto { get; init; }
}

public class CreateLichHocCommandHandler : IRequestHandler<CreateLichHocCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public CreateLichHocCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(CreateLichHocCommand request, CancellationToken cancellationToken)
    {
        var lichHocDto = request.LichHocDto;

        // Ensure Phòng exists
        var phong = await _context.Phongs.FindAsync(lichHocDto.PhongId);
        if (phong == null)
            throw new NotFoundDataException("Phòng không tồn tại.");

        // Ensure Giáo viên exists
        var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == lichHocDto.GiaoVienCode, cancellationToken);
        if (giaoVien == null)
            throw new NotFoundDataException("Giáo viên không tồn tại.");

        // Ensure Chương trình exists
        var chuongTrinh = await _context.ChuongTrinhs.FindAsync(lichHocDto.ChuongTrinhId);
        if (chuongTrinh == null)
            throw new NotFoundDataException("Chương trình không tồn tại.");

        // Parse time values
        if (!TimeOnly.TryParse(lichHocDto.GioBatDau, out var gioBatDau))
            throw new FormatException("Định dạng giờ bắt đầu không hợp lệ. Định dạng hợp lệ: HH:mm.");

        if (!TimeOnly.TryParse(lichHocDto.GioKetThuc, out var gioKetThuc))
            throw new FormatException("Định dạng giờ kết thúc không hợp lệ. Định dạng hợp lệ: HH:mm.");

        // Check for scheduling conflicts
        var hasConflict = await _context.LichHocs.AnyAsync(lh =>
            lh.Thu == lichHocDto.Thu &&
            lh.PhongId == lichHocDto.PhongId &&
            (gioBatDau < lh.GioKetThuc && gioKetThuc > lh.GioBatDau), cancellationToken);

        if (hasConflict)
            throw new Exception("Có lịch học trùng phòng, ngày, giờ với lớp khác.");

        // Create new LichHoc entity
        var lichHoc = new LichHoc
        {
            Id = Guid.NewGuid(),
            Thu = lichHocDto.Thu,
            PhongId = lichHocDto.PhongId,
            TenLop = lichHocDto.TenLop,
            GioBatDau = gioBatDau,
            GioKetThuc = gioKetThuc,
            NgayBatDau = lichHocDto.NgayBatDau,
            NgayKetThuc = lichHocDto.NgayKetThuc,
            HocPhi = lichHocDto.HocPhi,
            TrangThai = "NotYet",
            GiaoVienCode = lichHocDto.GiaoVienCode,
            ChuongTrinhId = lichHocDto.ChuongTrinhId
        };

        _context.LichHocs.Add(lichHoc);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = lichHoc,
            code = 200,
            message = "Thêm lịch học thành công."
        };
    }
}

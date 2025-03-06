using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public record EditLichHocCommand : IRequest<Output>
{
    public Guid Id { get; set; }
    public int? Thu { get; set; }
    public int? SlotId { get; set; }
    public string? Phong { get; set; }
    public string? TenLop { get; set; }
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


        // Validate - SlotId
        if (request.SlotId.HasValue)
        {
            var slotExists = await _context.Slots.AnyAsync(s => s.Id == request.SlotId.Value, cancellationToken);
            if (!slotExists)
            {
                throw new Exception("SlotId không hợp lệ.");
            }
        }

        // Validate - Phòng
        if (!string.IsNullOrWhiteSpace(request.Phong))
        {
            var phongExists = await _context.LichHocs.AnyAsync(p => p.Phong == request.Phong, cancellationToken);
            if (!phongExists)
            {
                throw new Exception("Phòng không hợp lệ.");
            }
        }

        // Validate - ChuongTrinhId
        if (request.ChuongTrinhId.HasValue)
        {
            var chuongTrinh = await _context.ChuongTrinhs.FindAsync(request.ChuongTrinhId.Value);
            if (chuongTrinh == null)
            {
                throw new NotFoundDataException("Chương trình không tồn tại.");
            }
        }

        // Validate - GiaoVienCode
        if (!string.IsNullOrWhiteSpace(request.GiaoVienCode))
        {
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == request.GiaoVienCode, cancellationToken);
            if (giaoVien == null)
            {
                throw new NotFoundDataException("Giáo viên không tồn tại.");
            }
        }

        //  Validate xem có trùng lịch không ?
        if (request.Thu.HasValue || request.SlotId.HasValue || !string.IsNullOrWhiteSpace(request.Phong) || request.ChuongTrinhId.HasValue)
        {
            var thu = request.Thu ?? lichHoc.Thu;
            var slotId = request.SlotId ?? lichHoc.SlotId;
            var phong = request.Phong ?? lichHoc.Phong;
            var chuongTrinhId = request.ChuongTrinhId ?? lichHoc.ChuongTrinhId;

            var conflict = await _context.LichHocs.AnyAsync(lh =>
                lh.Id != lichHoc.Id && 
                lh.Thu == thu &&
                lh.SlotId == slotId &&
                lh.Phong == phong &&
                lh.ChuongTrinhId == chuongTrinhId,
                cancellationToken);

            if (conflict)
            {
                throw new Exception("Có lịch học trùng với phòng, slot, thứ và chương trình này.");
            }
        }

        if (request.Thu.HasValue) lichHoc.Thu = request.Thu.Value;
        if (request.SlotId.HasValue) lichHoc.SlotId = request.SlotId.Value;
        if (!string.IsNullOrWhiteSpace(request.Phong)) lichHoc.Phong = request.Phong;
        if (!string.IsNullOrWhiteSpace(request.TenLop)) lichHoc.TenLop = request.TenLop;
        if (request.HocPhi.HasValue) lichHoc.HocPhi = request.HocPhi.Value;
        if (!string.IsNullOrWhiteSpace(request.TrangThai)) lichHoc.TrangThai = request.TrangThai;
        if (!string.IsNullOrWhiteSpace(request.GiaoVienCode))
        {
            lichHoc.GiaoVienCode = request.GiaoVienCode;
        }
        if (request.ChuongTrinhId.HasValue)
        {
            lichHoc.ChuongTrinhId = request.ChuongTrinhId.Value;
        }

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

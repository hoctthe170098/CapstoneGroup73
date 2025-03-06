using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
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

        if (request.Thu.HasValue) lichHoc.Thu = request.Thu.Value;
        if (!string.IsNullOrWhiteSpace(request.TenLop)) lichHoc.TenLop = request.TenLop;
        if (request.HocPhi.HasValue) lichHoc.HocPhi = request.HocPhi.Value;
        if (!string.IsNullOrWhiteSpace(request.TrangThai)) lichHoc.TrangThai = request.TrangThai;
        if (!string.IsNullOrWhiteSpace(request.GiaoVienCode))
        {
            lichHoc.GiaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == request.GiaoVienCode, cancellationToken)
                ?? throw new NotFoundDataException("Giáo viên không tồn tại.");
            lichHoc.GiaoVienCode = request.GiaoVienCode;
        }
        if (request.ChuongTrinhId.HasValue)
        {
            lichHoc.ChuongTrinh = await _context.ChuongTrinhs.FindAsync(request.ChuongTrinhId.Value)
                ?? throw new NotFoundDataException("Chương trình không tồn tại.");
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

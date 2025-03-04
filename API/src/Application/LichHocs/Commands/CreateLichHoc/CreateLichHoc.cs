using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;

public record CreateLichHocCommand : IRequest<Output>
{
    public int Thu { get; set; }
    public int SlotId { get; set; }
    public string Phong { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string NgayBatDau { get; set; } = string.Empty;
    public string? NgayKetThuc { get; set; }
    public int HocPhi { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string GiaoVienCode { get; set; } = string.Empty;
    public int ChuongTrinhId { get; set; }
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
        if (string.IsNullOrWhiteSpace(request.TenLop) || string.IsNullOrWhiteSpace(request.Phong))
        {
            throw new NotFoundDataException("Tên lớp và phòng không được để trống.");
        }

        var lichHoc = new LichHoc
        {
            Id = Guid.NewGuid(),
            Thu = request.Thu,
            SlotId = request.SlotId,
            Phong = request.Phong,
            TenLop = request.TenLop,
            NgayBatDau = request.NgayBatDau,
            NgayKetThuc = request.NgayKetThuc,
            HocPhi = request.HocPhi,
            TrangThai = request.TrangThai,
            GiaoVienCode = request.GiaoVienCode,
            ChuongTrinhId = request.ChuongTrinhId
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

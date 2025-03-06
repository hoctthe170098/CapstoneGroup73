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
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public int HocPhi { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string GiaoVienCode { get; set; } = string.Empty;
    public int ChuongTrinhId { get; set; }
    public string? CoSoId { get; set; }
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

        // Validate các trường không được bị để trống và không hợp lệ 
        if (string.IsNullOrWhiteSpace(request.TenLop) ||
            string.IsNullOrWhiteSpace(request.Phong) ||
            string.IsNullOrWhiteSpace(request.TrangThai) ||
            string.IsNullOrWhiteSpace(request.GiaoVienCode) ||
            request.Thu <= 0 ||
            request.SlotId <= 0 ||
            request.ChuongTrinhId <= 0 ||
            request.HocPhi < 0)

        if (string.IsNullOrWhiteSpace(request.TenLop) || string.IsNullOrWhiteSpace(request.Phong)
            ||string.IsNullOrWhiteSpace(request.CoSoId))
        {
            throw new NotFoundDataException("Các trường thông tin không được để trống hoặc không hợp lệ.");
        }

        // Kiểm tra trùng lịch: cùng thứ, SlotId và phòng
        var isDuplicated = _context.LichHocs.Any(lh =>
            lh.Thu == request.Thu &&
            lh.SlotId == request.SlotId &&
            lh.Phong == request.Phong);
            

        if (isDuplicated)
        {
            throw new Exception("Lịch học đã tồn tại cho phòng, slot và chương trình này.");
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
            ChuongTrinhId = request.ChuongTrinhId,
            CoSoId = Guid.Parse(request.CoSoId)
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

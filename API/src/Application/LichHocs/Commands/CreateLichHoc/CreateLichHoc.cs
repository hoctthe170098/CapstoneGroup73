using System.Text.Json.Serialization;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;

public record CreateLichHocCommand : IRequest<Output>
{
    public int Thu { get; set; }
    public int PhongId { get; set; }
    public string TenLop { get; set; } = string.Empty;
    public string GioBatDau { get; set; } = string.Empty;
    public string GioKetThuc { get; set; } = string.Empty;
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public int HocPhi { get; set; }
    public string TrangThai { get; set; } = "NotYet";
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
        if (string.IsNullOrWhiteSpace(request.TenLop))
            throw new WrongInputException("Tên lớp không được để trống.");

        if (string.IsNullOrWhiteSpace(request.TrangThai))
            throw new WrongInputException("Trạng thái không được để trống.");

        if (string.IsNullOrWhiteSpace(request.GiaoVienCode))
            throw new WrongInputException("Mã giáo viên không được để trống.");

        if (request.Thu < 2 || request.Thu > 8)
            throw new WrongInputException("Thứ không hợp lệ, phải từ thứ 2 đến Chủ nhật.");

        if (!TimeOnly.TryParse(request.GioBatDau, out var gioBatDau) || !TimeOnly.TryParse(request.GioKetThuc, out var gioKetThuc))
        {
            throw new WrongInputException("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");
        }

        if (gioBatDau >= gioKetThuc)
            throw new WrongInputException("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

        if (request.NgayBatDau > request.NgayKetThuc)
            throw new WrongInputException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

        if (request.HocPhi < 0)
            throw new WrongInputException("Học phí không hợp lệ.");

        var phong = await _context.Phongs.FindAsync(request.PhongId);
        if (phong == null)
        {
            throw new NotFoundDataException("Phòng không tồn tại.");
        }

        var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == request.GiaoVienCode, cancellationToken);
        if (giaoVien == null)
        {
            throw new NotFoundDataException("Giáo viên không tồn tại.");
        }

        var chuongTrinh = await _context.ChuongTrinhs.FindAsync(request.ChuongTrinhId);
        if (chuongTrinh == null)
        {
            throw new NotFoundDataException("Chương trình không tồn tại.");
        }

        var hasConflict = await _context.LichHocs.AnyAsync(lh =>
            lh.Thu == request.Thu &&
            lh.PhongId == request.PhongId &&
            (gioBatDau < lh.GioKetThuc && gioKetThuc > lh.GioBatDau), cancellationToken);

        if (hasConflict)
        {
            throw new Exception("Có lịch học trùng phòng, ngày, giờ với lớp khác.");
        }

        var lichHoc = new LichHoc
        {
            Id = Guid.NewGuid(),
            Thu = request.Thu,
            PhongId = request.PhongId,
            TenLop = request.TenLop,
            GioBatDau = gioBatDau,
            GioKetThuc = gioKetThuc,
            NgayBatDau = request.NgayBatDau,
            NgayKetThuc = request.NgayKetThuc,
            HocPhi = request.HocPhi,
            TrangThai = "NotYet",
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

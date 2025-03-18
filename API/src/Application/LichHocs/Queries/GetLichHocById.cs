using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.LichHocs.Queries;

public record GetLichHocByIdQuery(Guid Id) : IRequest<Output>;

public class GetLichHocByIdQueryHandler : IRequestHandler<GetLichHocByIdQuery, Output>
{
    private readonly IApplicationDbContext _context;

    public GetLichHocByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(GetLichHocByIdQuery request, CancellationToken cancellationToken)
    {
        var lichHoc = await _context.LichHocs
            .Include(lh => lh.Phong)  
            .Include(lh => lh.GiaoVien) 
            .Include(lh => lh.ChuongTrinh) 
            .FirstOrDefaultAsync(lh => lh.Id == request.Id, cancellationToken);

        if (lichHoc == null)
        {
            throw new NotFoundDataException($"Không tìm thấy lịch học với ID {request.Id}");
        }

        var result = new
        {
            lichHoc.Id,
            lichHoc.Thu,
            Phong = lichHoc.Phong?.Ten ?? "N/A",
            lichHoc.TenLop,
            GioBatDau = lichHoc.GioBatDau.ToString("HH:mm"),
            GioKetThuc = lichHoc.GioKetThuc.ToString("HH:mm"),
            NgayBatDau = lichHoc.NgayBatDau.ToString("yyyy-MM-dd"),
            NgayKetThuc = lichHoc.NgayKetThuc.ToString("yyyy-MM-dd"),
            lichHoc.HocPhi,
            lichHoc.TrangThai,
            GiaoVien = lichHoc.GiaoVien?.Ten ?? "N/A",
            ChuongTrinh = lichHoc.ChuongTrinh?.TieuDe ?? "N/A"
        };

        return new Output
        {
            isError = false,
            data = result,
            code = 200,
            message = "Lấy lịch học thành công."
        };
    }
}

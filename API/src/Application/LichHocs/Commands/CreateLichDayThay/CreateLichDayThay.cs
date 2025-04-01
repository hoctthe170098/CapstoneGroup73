using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichDayThay;
public record CreateLichDayThayCommand : IRequest<Output>
{
    public required string TenLop {  get; init; }
    public required DateOnly NgayDay { get; init; }
    public required string GiaoVienCode {  get; init; }
}
    public class CreateLichDayThayCommandHandler : IRequestHandler<CreateLichDayThayCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateLichDayThayCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(CreateLichDayThayCommand request, CancellationToken cancellationToken)
        {
        var thu = ((int)request.NgayDay.DayOfWeek > 0)
        ? (int)request.NgayDay.DayOfWeek + 1
        : (int)request.NgayDay.DayOfWeek + 8;
        var lichHoc = await _context.LichHocs
            .FirstAsync(lh=>lh.Thu == thu&&lh.TenLop==request.TenLop);
        var lichDayThay = new LichHoc
        {
            Id = Guid.NewGuid(),
            ChuongTrinhId = lichHoc.ChuongTrinhId,
            GiaoVienCode = request.GiaoVienCode,
            GioBatDau = lichHoc.GioBatDau,
            GioKetThuc = lichHoc.GioKetThuc,
            HocPhi = lichHoc.HocPhi,
            NgayBatDau = request.NgayDay,
            NgayKetThuc = request.NgayDay,
            PhongId = lichHoc.PhongId,
            TenLop = request.TenLop,
            Thu = thu,
            TrangThai = "Dạy thay",
            LichHocGocId = lichHoc.Id,
            NgayHocGoc = request.NgayDay
        };
        _context.LichHocs.Add(lichDayThay);
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            code = 200,
            isError = false,
            data = lichDayThay,
            message = "Tạo lịch dạy thay thành công."
        };
        }
    }

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.UpdateLichDayBu;
public record UpdateLichDayBuCommand : IRequest<Output>
{
    public required Guid Id { get; init; }
    public required string TenLop {  get; init; }
    public required DateOnly NgayNghi { get; init; }
    public required LichDayBuDto LichDayBu { get; init; }
}
    public class UpdateLichDayBuCommandHandler : IRequestHandler<UpdateLichDayBuCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateLichDayBuCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(UpdateLichDayBuCommand request, CancellationToken cancellationToken)
        {
        var thuHocBu = ((int)request.LichDayBu.NgayHocBu.DayOfWeek > 0)
        ? (int)request.LichDayBu.NgayHocBu.DayOfWeek + 1
        : (int)request.LichDayBu.NgayHocBu.DayOfWeek + 8;
        var lichHocBu = await _context.LichHocs.FirstAsync(lh=>lh.Id == request.Id);
        lichHocBu.Thu = thuHocBu;
        lichHocBu.PhongId = request.LichDayBu.PhongId;
        lichHocBu.GioBatDau = TimeOnly.Parse(request.LichDayBu.GioBatDau);
        lichHocBu.GioKetThuc = TimeOnly.Parse(request.LichDayBu.GioKetThuc);
        lichHocBu.NgayBatDau = request.LichDayBu.NgayHocBu;
        lichHocBu.NgayKetThuc = request.LichDayBu.NgayHocBu;
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            code = 200,
            isError = false,
            data = null,
            message = "Chỉnh sửa lịch dạy bù thành công."
        };
        }
    }

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.UpdateLichDayThay;
public record UpdateLichDayThayCommand : IRequest<Output>
{
    public required Guid Id { get; set; }
    public required string TenLop {  get; init; }
    public required DateOnly NgayDay { get; init; }
    public required string GiaoVienCode {  get; init; }
}
    public class UpdateLichDayThayCommandHandler : IRequestHandler<UpdateLichDayThayCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateLichDayThayCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(UpdateLichDayThayCommand request, CancellationToken cancellationToken)
        {
        var thu = ((int)request.NgayDay.DayOfWeek > 0)
        ? (int)request.NgayDay.DayOfWeek + 1
        : (int)request.NgayDay.DayOfWeek + 8;
        var lichHoc = await _context.LichHocs
            .FirstAsync(lh=>lh.Thu == thu&&lh.TenLop==request.TenLop);
        var lichDayThayHienTai = _context.LichHocs.Where(lh=>lh.Id==request.Id).First();
        lichDayThayHienTai.NgayBatDau = request.NgayDay;
        lichDayThayHienTai.NgayKetThuc = request.NgayDay;
        lichDayThayHienTai.GiaoVienCode = request.GiaoVienCode;
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            code = 200,
            isError = false,
            data = lichDayThayHienTai,
            message = "Thay đổi lịch dạy thay thành công."
        };
        }
    }

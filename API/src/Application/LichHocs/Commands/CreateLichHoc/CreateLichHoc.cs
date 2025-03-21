using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
public record CreateLichHocCommand : IRequest<Output>
{
    public required CreateLichHocDto LopHocDto { get; init; }
}
    public class CreateLichHocCommandHandler : IRequestHandler<CreateLichHocCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateLichHocCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(CreateLichHocCommand request, CancellationToken cancellationToken)
        {
            var lopHocDto = request.LopHocDto;
            foreach (var lichHocDto in lopHocDto.LichHocs)
        {
            LichHoc lichHoc = new LichHoc
            {
                Id = Guid.NewGuid(),
                ChuongTrinhId = lopHocDto.ChuongTrinhId,
                GiaoVienCode = lopHocDto.GiaoVienCode,
                GioBatDau = TimeOnly.Parse(lichHocDto.GioBatDau),
                GioKetThuc = TimeOnly.Parse(lichHocDto.GioKetThuc),
                HocPhi = lopHocDto.HocPhi,
                NgayBatDau = lopHocDto.NgayBatDau,
                NgayKetThuc = lopHocDto.NgayKetThuc,
                PhongId = lichHocDto.PhongId,
                TenLop = lopHocDto.TenLop,
                Thu = lichHocDto.Thu,
                TrangThai = "Cố định"
            };
            await _context.LichHocs.AddAsync(lichHoc,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return new Output
        {
            code = 200,
            isError = false,
            data = lopHocDto,
            message = "Tạo mới lớp học thành công"
        };
        }
    }

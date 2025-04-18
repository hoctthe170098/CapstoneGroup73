using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanh;
using StudyFlow.Domain.Entities;


namespace StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanh;
public record UpdateDiemDanhCommand : IRequest<Output>
{
    public required List<UpdateDiemDanhDto> UpdateDiemDanhs { get; init; }
}
    public class UpdateDiemDanhCommandHandler : IRequestHandler<UpdateDiemDanhCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateDiemDanhCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(UpdateDiemDanhCommand request, CancellationToken cancellationToken)
        {
           foreach(var item in request.UpdateDiemDanhs)
        {
            var diemDanh = _context.DiemDanhs.First(dd=>dd.Id== item.Id);
            diemDanh.TrangThai = item.TrangThai;
            await _context.SaveChangesAsync(cancellationToken);
        }
        return new Output
        {
            isError = false,
            code = 200,
            message = "Chỉnh sửa điểm danh thành công"
        };
        }
    }

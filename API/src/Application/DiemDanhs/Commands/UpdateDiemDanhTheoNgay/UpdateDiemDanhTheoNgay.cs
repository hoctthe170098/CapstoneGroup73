using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay;
using StudyFlow.Domain.Entities;


namespace StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay;
public record UpdateDiemDanhTheoNgayCommand : IRequest<Output>
{
    public required List<UpdateDiemDanhDto> UpdateDiemDanhs { get; init; }
}
    public class UpdateDiemDanhTheoNgayCommandHandler : IRequestHandler<UpdateDiemDanhTheoNgayCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateDiemDanhTheoNgayCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(UpdateDiemDanhTheoNgayCommand request, CancellationToken cancellationToken)
        {
           foreach(var item in request.UpdateDiemDanhs)
        {
            var diemDanh = _context.DiemDanhs.First(dd=>dd.Id== item.Id);
            diemDanh.TrangThai = item.TrangThai;
            diemDanh.DiemBTVN = item.DiemBTVN;
            diemDanh.DiemTrenLop = item.DiemTrenLop;
            diemDanh.NhanXet = item.NhanXet;
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

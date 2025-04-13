using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;


namespace StudyFlow.Application.KetQuaBaiKiemTras.Commands.UpdateKetQuaBaiKiemTra;
public record UpdateKetQuaBaiKiemTraCommand : IRequest<Output>
{
    public required List<KetQuaBaiKiemTraDto> UpdateKetQuas { get; init; }
}
    public class UpdateKetQuaBaiKiemTraCommandHandler : IRequestHandler<UpdateKetQuaBaiKiemTraCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateKetQuaBaiKiemTraCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(UpdateKetQuaBaiKiemTraCommand request, CancellationToken cancellationToken)
        {
           foreach(var item in request.UpdateKetQuas)
        {
            var ketQua = _context.KetQuaBaiKiemTras.First(kq=>kq.Id == item.Id);
            ketQua.Diem = item.Diem;
            ketQua.NhanXet = item.NhanXet;
            await _context.SaveChangesAsync(cancellationToken);
        }
        return new Output
        {
            isError = false,
            code = 200,
            message = "Chỉnh sửa kết quá bài kiểm tra thành công"
        };
        }
    }

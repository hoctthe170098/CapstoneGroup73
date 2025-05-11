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
    public required DateOnly Ngay { get; init; }
    public required string TenLop {  get; init; }
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
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        foreach (var item in request.UpdateDiemDanhs)
        {
            if (item.Id != null)
            {
                var diemDanh = _context.DiemDanhs.First(dd => dd.Id == item.Id);
                diemDanh.TrangThai = item.TrangThai;
            }
            else 
            {
                if (item.TrangThai == "Không điểm danh") continue;
                int thu = (int)(request.Ngay.DayOfWeek);
                if (thu == 0) thu = 8; else thu++;
                var lichDayBu = _context.LichHocs
                    .FirstOrDefault(lh => lh.TenLop == request.TenLop 
                    && lh.Phong.CoSoId == coSoId 
                    && lh.NgayBatDau == request.Ngay 
                    && lh.NgayKetThuc == request.Ngay 
                    && lh.TrangThai == "Dạy bù");
                if (lichDayBu != null)
                {
                    var ThamGia = _context.ThamGiaLopHocs
                    .First(tg => tg.HocSinhCode == item.HocSinhCode && tg.LichHocId == lichDayBu.Id);
                    var diemDanh = new DiemDanh
                    {
                        Id = Guid.NewGuid(),
                        ThamGiaLopHocId = ThamGia.Id,
                        TrangThai = item.TrangThai,
                        Ngay = request.Ngay
                    };
                    _context.DiemDanhs.Add(diemDanh);
                }
                else 
                {
                    var lichHoc = _context.LichHocs.First(lh => lh.Thu == thu
                                        && lh.TenLop == request.TenLop && lh.TrangThai=="Cố định");
                    var ThamGia = _context.ThamGiaLopHocs
                        .First(tg => tg.HocSinhCode == item.HocSinhCode && tg.LichHocId == lichHoc.Id);
                    var diemDanh = new DiemDanh
                    {
                        Id = Guid.NewGuid(),
                        ThamGiaLopHocId = ThamGia.Id,
                        TrangThai = item.TrangThai,
                        Ngay = request.Ngay
                    };
                    _context.DiemDanhs.Add(diemDanh);
                }              
            }
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

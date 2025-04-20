using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
using StudyFlow.Domain.Entities;



namespace StudyFlow.Application.NhanXetDinhKys.Commands.UpdateNhanXetDinhKy;
public record UpdateNhanXetDinhKyCommand : IRequest<Output>
{
    public required Guid Id { get; set; }
    public required string NoiDungNhanXet { get; set; }
}
public class UpdateNhanXetDinhKyCommandHandler : IRequestHandler<UpdateNhanXetDinhKyCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateNhanXetDinhKyCommandHandler(IApplicationDbContext context, IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Output> Handle(UpdateNhanXetDinhKyCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var NhanXetDinhKy = _context.NhanXetDinhKys
            .Include(nv=>nv.ThamGiaLopHoc)
            .ThenInclude(tg=>tg.LichHoc)
            .Include(nv=>nv.ThamGiaLopHoc)
            .ThenInclude(nv=>nv.HocSinh)
            .First(nv=>nv.Id==request.Id);
        var HocSinhCode = NhanXetDinhKy.ThamGiaLopHoc.HocSinh.Code;
        var TenLop = NhanXetDinhKy.ThamGiaLopHoc.LichHoc.TenLop;
        var ThamGiaLopHocs = await _context.ThamGiaLopHocs
            .Where(tg => tg.HocSinhCode == HocSinhCode
            && tg.LichHoc.TenLop == TenLop
            && tg.LichHoc.Phong.CoSoId == coSoId
            && tg.LichHoc.TrangThai == "Cố định")
            .Include(tg => tg.LichHoc)
            .Include(tg => tg.HocSinh)
            .ToListAsync();
        var NhanXetDinhKys = _context.NhanXetDinhKys
            .Where(nx => ThamGiaLopHocs.Select(tg => tg.Id).Contains(nx.ThamGiaLopHocId))
            .OrderBy(nx => nx.STT)
            .ToList();
        if (NhanXetDinhKy.STT==NhanXetDinhKys.Count)
        {
            NhanXetDinhKy.NoiDungNhanXet = request.NoiDungNhanXet;
            await _context.SaveChangesAsync(cancellationToken);
            return new Output
            {
                isError = false,
                code = 200,
                message = "Chỉnh sửa nhận xét định kỳ thành công."
            };
        }
        else throw new Exception("Đã hết hạn chỉnh sửa nhận xét định kỳ.");
    }
}

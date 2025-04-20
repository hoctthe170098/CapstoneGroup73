using CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.NhanXetDinhKys.Commands.DeleteNhanXetDinhKy;

public record DeleteNhanXetDinhKyCommand(Guid Id) : IRequest<Output>;

public class DeleteNhanXetDinhKyCommandHandler : IRequestHandler<DeleteNhanXetDinhKyCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public DeleteNhanXetDinhKyCommandHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(DeleteNhanXetDinhKyCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var UserId = _identityService.GetUserId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var giaoVien = await _context.GiaoViens
            .FirstAsync(gv => gv.UserId == UserId.ToString());
        var NhanXetDinhKy = await _context.NhanXetDinhKys
            .Include(nv => nv.ThamGiaLopHoc)
            .ThenInclude(tg => tg.LichHoc)
            .Include(nv => nv.ThamGiaLopHoc)
            .ThenInclude(nv => nv.HocSinh)
            .FirstOrDefaultAsync(x => x.Id == request.Id 
            && x.ThamGiaLopHoc.LichHoc.GiaoVienCode == giaoVien.Code);
        if(NhanXetDinhKy==null) throw new NotFoundIDException();
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
        if (NhanXetDinhKy.STT == NhanXetDinhKys.Count)
        {
            _context.NhanXetDinhKys.Remove(NhanXetDinhKy);
            await _context.SaveChangesAsync(cancellationToken);
            return new Output
            {
                isError = false,
                code = 200,
                message = "Xoá nhận xét định kỳ thành công."
            };
        }
        else throw new Exception("Đã hết hạn xoá nhận xét định kỳ.");
    }
}

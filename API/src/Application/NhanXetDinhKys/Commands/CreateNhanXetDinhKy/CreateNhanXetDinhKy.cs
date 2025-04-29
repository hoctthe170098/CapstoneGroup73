using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
using StudyFlow.Domain.Entities;



namespace StudyFlow.Application.NhanXetDinhKys.Commands.CreateNhanXetDinhKy;
public record CreateNhanXetDinhKyCommand : IRequest<Output>
{
    public required string HocSinhCode { get; set; }
    public required string TenLop { get; set; }
    public required string NoiDungNhanXet { get; set; }
}
public class CreateNhanXetDinhKyCommandHandler : IRequestHandler<CreateNhanXetDinhKyCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateNhanXetDinhKyCommandHandler(IApplicationDbContext context, IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Output> Handle(CreateNhanXetDinhKyCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var UserId = _identityService.GetUserId(token);
        var giaoVien = await _context.GiaoViens
            .FirstAsync(gv => gv.UserId == UserId.ToString());
        var checkLopHoc = _context.LichHocs
            .Any(lh => lh.GiaoVienCode == giaoVien.Code && lh.TrangThai == "Cố định" && lh.TenLop == request.TenLop);
        if (!checkLopHoc) throw new NotFoundIDException();
        var ThamGiaLopHocs = await _context.ThamGiaLopHocs
            .Where(tg => tg.HocSinhCode == request.HocSinhCode
            && tg.LichHoc.TenLop == request.TenLop
            && tg.LichHoc.Phong.CoSoId == coSoId
            && tg.LichHoc.TrangThai == "Cố định")
            .Include(tg => tg.LichHoc)
            .Include(tg => tg.HocSinh)
            .ToListAsync();
        var NgayDaHoc = await _context.DiemDanhs
            .Where(dd => ThamGiaLopHocs.Select(tg => tg.Id).Contains(dd.ThamGiaLopHocId))
            .Select(dd => dd.Ngay)
            .Distinct()
            .ToListAsync();
        var NhanXetDinhKys = _context.NhanXetDinhKys
            .Where(nx => ThamGiaLopHocs.Select(tg => tg.Id).Contains(nx.ThamGiaLopHocId))
            .OrderBy(nx => nx.STT)
            .ToList();
        var ThamGiaLopHocHienTai = ThamGiaLopHocs.OrderByDescending(tg => tg.NgayKetThuc).ToList()[0];
        if (NgayDaHoc.Count / 4 > NhanXetDinhKys.Count)
        {
            var nhanXetDinhKy = new NhanXetDinhKy
            {
                Id = Guid.NewGuid(),
                NgayNhanXet = DateOnly.FromDateTime(DateTime.Now),
                NoiDungNhanXet = request.NoiDungNhanXet,
                ThamGiaLopHocId = ThamGiaLopHocHienTai.Id,
                STT = NhanXetDinhKys.Count + 1
            };
            _context.NhanXetDinhKys.Add(nhanXetDinhKy);
            await _identityService.SendNhanXetDinhKy(request.HocSinhCode,request.TenLop,nhanXetDinhKy);
            await _context.SaveChangesAsync(cancellationToken);
            return new Output
            {
                isError = false,
                code = 200,
                message = "Thêm nhận xét định kỳ thành công."
            };
        }
        else throw new Exception("Chưa đến hạn nhận xét định kỳ(4 buổi 1 lần).");
    }
}

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
        var ThamGiaLopHoc = await _context.ThamGiaLopHocs
            .Where(tg => tg.HocSinhCode == request.HocSinhCode
            && tg.LichHoc.TenLop == request.TenLop
            && tg.LichHoc.Phong.CoSoId == coSoId
            && tg.LichHoc.TrangThai == "Cố định")
            .Include(tg => tg.LichHoc)
            .Include(tg => tg.HocSinh)
            .ToListAsync();
        var LichHocCoDinh = ThamGiaLopHoc.Select(tg => tg.LichHoc).ToList();
        if (!LichHocCoDinh.Any()) throw new NotFoundIDException();
        DateOnly ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
        var Thus = LichHocCoDinh.Select(lh => lh.Thu).ToList();
        var NgayKetThuc = ThamGiaLopHoc[0].NgayKetThuc;
        if (NgayKetThuc > ngayHienTai) NgayKetThuc = ngayHienTai;
        var NgayDaHoc = getNgayDaHoc(Thus, ThamGiaLopHoc[0].NgayBatDau, NgayKetThuc);
        var listHocBu = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop
            && lh.Phong.CoSoId == coSoId
            && lh.TrangThai == "Học bù")
            .ToList();
        foreach (var hocBu in listHocBu)
        {
            if (hocBu.NgayHocGoc < NgayKetThuc) NgayDaHoc.Remove((DateOnly)hocBu.NgayHocGoc);
            if (hocBu.NgayBatDau < NgayKetThuc) NgayDaHoc.Add(hocBu.NgayBatDau);
        }
        var NhanXetDinhKys = _context.NhanXetDinhKys
            .Where(nx => ThamGiaLopHoc.Select(tg => tg.Id).Contains(nx.ThamGiaLopHocId))
            .OrderBy(nx => nx.STT)
            .ToList();
        if (NgayDaHoc.Count / 4 > NhanXetDinhKys.Count)
        {
            var nhanXetDinhKy = new NhanXetDinhKy
            {
                Id = Guid.NewGuid(),
                NgayNhanXet = DateOnly.FromDateTime(DateTime.Now),
                NoiDungNhanXet = request.NoiDungNhanXet,
                ThamGiaLopHocId = ThamGiaLopHoc[0].Id,
                STT = NhanXetDinhKys.Count + 1
            };
            _context.NhanXetDinhKys.Add(nhanXetDinhKy);
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
    private List<DateOnly> getNgayDaHoc(List<int> Thus, DateOnly ngayBatDau,
   DateOnly ngayHienTai)
    {
        List<DateOnly> ngayDaHocList = new List<DateOnly>();

        for (DateOnly ngay = ngayBatDau; ngay <= ngayHienTai; ngay = ngay.AddDays(1))
        {
            int thu = (int)ngay.DayOfWeek == 0 ? 8 : (int)ngay.DayOfWeek + 1;
            if (Thus.Contains(thu))
            {
                ngayDaHocList.Add(ngay);
            }
        }
        return ngayDaHocList;
    }
}

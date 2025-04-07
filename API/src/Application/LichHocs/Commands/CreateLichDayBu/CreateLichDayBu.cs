using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichDayBu;
public record CreateLichDayBuCommand : IRequest<Output>
{
    public required string TenLop {  get; init; }
    public required DateOnly NgayNghi { get; init; }
    public LichDayBuDto? LichDayBu { get; init; }
}
    public class CreateLichDayBuCommandHandler : IRequestHandler<CreateLichDayBuCommand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateLichDayBuCommandHandler(IApplicationDbContext context, IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Output> Handle(CreateLichDayBuCommand request, CancellationToken cancellationToken)
        {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var thu = ((int)request.NgayNghi.DayOfWeek > 0)
        ? (int)request.NgayNghi.DayOfWeek + 1
        : (int)request.NgayNghi.DayOfWeek + 8;
        var lichDayThem = _context.LichHocs
            .FirstOrDefault(lh => lh.NgayHocGoc == request.NgayNghi
            && lh.TrangThai == "Dạy thay"
            && lh.Phong.CoSoId == coSoId);
        if (lichDayThem != null)
        _context.LichHocs.Remove(lichDayThem);
        var lichHoc = await _context.LichHocs
            .FirstAsync(lh=>lh.Thu == thu 
            && lh.TenLop == request.TenLop 
            && lh.TrangThai == "Cố định" 
            && lh.Phong.CoSoId == coSoId);
        if (request.LichDayBu == null)
        {
            var lichDayBu = new LichHoc
            {
                Id = Guid.NewGuid(),
                ChuongTrinhId = lichHoc.ChuongTrinhId,
                GiaoVienCode = lichHoc.GiaoVienCode,
                GioBatDau = lichHoc.GioBatDau,
                GioKetThuc = lichHoc.GioKetThuc,
                HocPhi = lichHoc.HocPhi,
                NgayBatDau = DateOnly.MinValue,
                NgayKetThuc = DateOnly.MinValue,
                PhongId = lichHoc.PhongId,
                TenLop = lichHoc.TenLop,
                Thu = lichHoc.Thu,
                TrangThai = "Dạy bù",
                LichHocGocId = lichHoc.Id,
                NgayHocGoc = request.NgayNghi,
            };
            _context.LichHocs.Add(lichDayBu);
            await _context.SaveChangesAsync(cancellationToken);
            return new Output
            {
                code = 200,
                isError = false,
                data = null,
                message = "Đã cho nghỉ buổi học thành công."
            };
        }
        else
        {
            var thuHocBu = ((int)request.LichDayBu.NgayHocBu.DayOfWeek > 0)
        ? (int)request.LichDayBu.NgayHocBu.DayOfWeek + 1
        : (int)request.LichDayBu.NgayHocBu.DayOfWeek + 8;
            var lichDayBu = _context.LichHocs
                .FirstOrDefault(lh => lh.TenLop == request.TenLop
                && lh.TrangThai == "Dạy bù"
                && lh.NgayHocGoc == request.NgayNghi
                && lh.NgayKetThuc == DateOnly.MinValue
                 && lh.Phong.CoSoId == coSoId);
            if(lichDayBu != null)
            {
                lichDayBu.Thu = thuHocBu;
                lichDayBu.PhongId = request.LichDayBu.PhongId;
                lichDayBu.GioBatDau = TimeOnly.Parse(request.LichDayBu.GioBatDau);
                lichDayBu.GioKetThuc = TimeOnly.Parse(request.LichDayBu.GioKetThuc);
                lichDayBu.NgayBatDau = request.LichDayBu.NgayHocBu;
                lichDayBu.NgayKetThuc = request.LichDayBu.NgayHocBu;
            }
            else 
            {
                lichDayBu = new LichHoc
                {
                    Id = Guid.NewGuid(),
                    ChuongTrinhId = lichHoc.ChuongTrinhId,
                    GiaoVienCode = lichHoc.GiaoVienCode,
                    GioBatDau = TimeOnly.Parse(request.LichDayBu.GioBatDau),
                    GioKetThuc = TimeOnly.Parse(request.LichDayBu.GioKetThuc),
                    HocPhi = lichHoc.HocPhi,
                    NgayBatDau = request.LichDayBu.NgayHocBu,
                    NgayKetThuc = request.LichDayBu.NgayHocBu,
                    PhongId = request.LichDayBu.PhongId,
                    TenLop = lichHoc.TenLop,
                    Thu = thuHocBu,
                    TrangThai = "Dạy bù",
                    LichHocGocId = lichHoc.Id,
                    NgayHocGoc = request.NgayNghi
                };
                _context.LichHocs.Add(lichDayBu);
            }
            var lichCoDinh = _context.LichHocs
            .Where(lh => lh.TenLop == request.TenLop && lh.TrangThai == "Cố định" && lh.Phong.CoSoId == coSoId)
            .Select(lh => lh.Id)
            .ToList();
            var hocSinhCodes = _context.ThamGiaLopHocs
            .Where(tg => lichCoDinh.Contains(tg.LichHocId))
            .Select(tg => tg.HocSinhCode)
            .Distinct()
            .ToList();
            foreach(var hs in hocSinhCodes)
            {
                var thamGia = new ThamGiaLopHoc
                {
                    Id = Guid.NewGuid(),
                    NgayBatDau = lichDayBu.NgayBatDau,
                    NgayKetThuc = lichDayBu.NgayKetThuc,
                    HocSinhCode = hs,
                    LichHocId = lichDayBu.Id,
                    TrangThai = "Học bù",
                };
                _context.ThamGiaLopHocs.Add(thamGia);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }     
        return new Output
        {
            code = 200,
            isError = false,
            data = null,
            message = "Tạo lịch dạy bù thành công."
        };
        }
    }

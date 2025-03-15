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
    public required CreateLichHocDto LichHocDto { get; init; }
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
            var lichHocDto = request.LichHocDto;

            // Extract token
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");

            // Get CoSoId from JWT token
            var coSoId =  _identityService.GetCampusId(token);

            // Ensure Phòng exists and belongs to the user's CoSoId
            var phong = await _context.Phongs.FirstOrDefaultAsync(p => p.Id == lichHocDto.PhongId && p.CoSoId == coSoId);
            if (phong == null)
                throw new NotFoundDataException("Phòng không tồn tại hoặc không thuộc cơ sở của bạn.");

            // Ensure Giáo viên exists
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.Code == lichHocDto.GiaoVienCode, cancellationToken);
            if (giaoVien == null)
                throw new NotFoundDataException("Giáo viên không tồn tại.");

            // Ensure Chương trình exists
            var chuongTrinh = await _context.ChuongTrinhs.FindAsync(lichHocDto.ChuongTrinhId);
            if (chuongTrinh == null)
                throw new NotFoundDataException("Chương trình không tồn tại.");

            // Validate dates
            if (!DateOnly.TryParseExact(lichHocDto.NgayBatDau, "yyyy-MM-dd", out var ngayBatDau))
                throw new FormatException("Ngày Bắt Đầu không hợp lệ. Định dạng phải là yyyy-MM-dd.");

            if (!DateOnly.TryParseExact(lichHocDto.NgayKetThuc, "yyyy-MM-dd", out var ngayKetThuc))
                throw new FormatException("Ngày Kết Thúc không hợp lệ. Định dạng phải là yyyy-MM-dd.");

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (ngayBatDau < today)
                throw new Exception("Ngày Bắt Đầu phải từ hôm nay trở đi.");

            var minEndDate = ngayBatDau.AddMonths(2);
            if (ngayKetThuc < minEndDate)
                throw new Exception("Ngày Kết Thúc phải ít nhất 2 tháng sau Ngày Bắt Đầu.");

            // Validate time
            if (!TimeOnly.TryParse(lichHocDto.GioBatDau, out var gioBatDau) || !TimeOnly.TryParse(lichHocDto.GioKetThuc, out var gioKetThuc))
                throw new FormatException("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");

            if (gioBatDau >= gioKetThuc)
                throw new Exception("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

            // Check for scheduling conflicts
            var hasConflict = await _context.LichHocs.AnyAsync(lh =>
                lh.Thu == lichHocDto.Thu &&
                lh.PhongId == lichHocDto.PhongId &&
                (gioBatDau < lh.GioKetThuc && gioKetThuc > lh.GioBatDau), cancellationToken);

            if (hasConflict)
                throw new Exception("Có lịch học trùng phòng, ngày, giờ với lớp khác.");

            var lichHoc = new LichHoc
            {
                Id = Guid.NewGuid(),
                Thu = lichHocDto.Thu,
                PhongId = lichHocDto.PhongId,
                TenLop = lichHocDto.TenLop,
                GioBatDau = gioBatDau,
                GioKetThuc = gioKetThuc,
                NgayBatDau = ngayBatDau,
                NgayKetThuc = ngayKetThuc,
                HocPhi = lichHocDto.HocPhi,
                TrangThai = "NotYet",
                GiaoVienCode = lichHocDto.GiaoVienCode,
                ChuongTrinhId = lichHocDto.ChuongTrinhId
            };

            _context.LichHocs.Add(lichHoc);
            await _context.SaveChangesAsync(cancellationToken);

            return new Output
            {
                isError = false,
                data = lichHoc,
                code = 200,
                message = "Thêm lịch học thành công."
            };
        }
    }

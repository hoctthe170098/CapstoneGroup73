using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public class EditLichHocCommandValidator : AbstractValidator<EditLichHocCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    public EditLichHocCommandValidator(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor
        , IIdentityService identityService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        RuleFor(x => x.LopHocDto.TenLop)
            .MustAsync(ExistClassName)
            .WithMessage("Lớp đã kết thúc và không thể chỉnh sửa");
        RuleFor(x => x.LopHocDto.LichHocs)
            .NotEmpty().WithMessage("Danh sách Lịch Học không được để trống")
            .MustAsync(ChuaBatDau)
            .WithMessage("Lớp học đã bắt đầu, không thể update lịch học")
            .MustAsync(ValidLichHoc)
            .WithMessage("Danh sách Lịch Học đã bị trùng hoặc không hợp lệ" +
            "(thời gian nghỉ giữa 2 tiết của 1 giáo viên ít nhất là 15p)");
        RuleFor(x => x.LopHocDto.HocSinhCodes)
            .Must(EnoughHocSinh)
            .WithMessage("Lớp học phải có ít nhất 3 học sinh")
            .MustAsync(ExistHocSinh)
            .WithMessage("Có học sinh không tồn tại trong cơ sở này")
            .MustAsync(ValidLichHocHocSinh)
            .WithMessage("Có học sinh bị trùng lịch");
    }

    private async Task<bool> ValidLichHocHocSinh(EditLichHocCommand command
        , List<string> hocSinhCodes, CancellationToken cToken)
    {
        List<string> invalidCodes = new List<string>();
        //var command = context.InstanceToValidate; // Lấy EditLichHocCommand từ context
        var lichHocDtos = command.LopHocDto.LichHocs; // Lấy danh sách lịch học mới
        var ngayBatDauLop = command.LopHocDto.NgayBatDau;
        var ngayKetThucLop = command.LopHocDto.NgayKetThuc;

        // Bước 1: Lấy tất cả lịch học hiện tại của các học sinh trong hocSinhCodes
        var thamGiaLopHocs = await _context.ThamGiaLopHocs
            .Include(tg => tg.LichHoc)
            .Where(tg => hocSinhCodes.Contains(tg.HocSinhCode))
            .ToListAsync(cToken);

        // Bước 2: Kiểm tra từng học sinh
        foreach (var hocSinhCode in hocSinhCodes)
        {
            // Lấy tất cả lịch học hiện tại của học sinh này
            var lichHocHienTais = thamGiaLopHocs
                .Where(tg => tg.HocSinhCode == hocSinhCode)
                .Select(tg => new
                {
                    LichHoc = tg.LichHoc,
                    NgayBatDauThamGia = tg.NgayBatDau,
                    NgayKetThucThamGia = tg.NgayKetThuc
                })
                .ToList();

            // Bước 3: So sánh với từng lịch học mới trong LichHocDtos
            foreach (var lichHocDto in lichHocDtos)
            {
                var gioBatDauMoi = TimeOnly.Parse(lichHocDto.GioBatDau);
                var gioKetThucMoi = TimeOnly.Parse(lichHocDto.GioKetThuc);
                var thuMoi = lichHocDto.Thu;

                foreach (var lichHocHienTai in lichHocHienTais)
                {
                    var lichHoc = lichHocHienTai.LichHoc;
                    var ngayBatDauHienTai = lichHoc.NgayBatDau > lichHocHienTai.NgayBatDauThamGia 
                        ? lichHoc.NgayBatDau : lichHocHienTai.NgayBatDauThamGia;
                    var ngayKetThucHienTai = lichHoc.NgayKetThuc < lichHocHienTai.NgayKetThucThamGia 
                        ? lichHoc.NgayKetThuc : lichHocHienTai.NgayKetThucThamGia;

                    // Kiểm tra xem hai lịch học có giao nhau về khoảng ngày không
                    if (ngayKetThucHienTai < ngayBatDauLop || ngayBatDauHienTai > ngayKetThucLop)
                    {
                        continue; // Không giao nhau về khoảng ngày, bỏ qua
                    }

                    // Kiểm tra xem hai lịch học có cùng ngày (cùng thứ) không
                    if (lichHoc.Thu != thuMoi)
                    {
                        continue; // Không cùng thứ, không cần kiểm tra
                    }

                    // So sánh thời gian
                    var gioBatDauHienTai = lichHoc.GioBatDau;
                    var gioKetThucHienTai = lichHoc.GioKetThuc;

                    // Kiểm tra giao nhau về thời gian
                    if (gioBatDauMoi < gioKetThucHienTai && gioKetThucMoi > gioBatDauHienTai)
                    {
                        invalidCodes.Add(hocSinhCode);
                        break; // Đã trùng lịch, không cần kiểm tra thêm
                    }

                    // Kiểm tra khoảng nghỉ 15 phút
                    var thoiGianNghi = TimeSpan.FromMinutes(15);
                    if (gioKetThucMoi <= gioBatDauHienTai)
                    {
                        // Lịch mới kết thúc trước lịch hiện tại
                        var thoiGianNghiThucTe = gioBatDauHienTai.ToTimeSpan() - gioKetThucMoi.ToTimeSpan();
                        if (thoiGianNghiThucTe < thoiGianNghi)
                        {
                            invalidCodes.Add(hocSinhCode);
                            break;
                        }
                    }
                    else if (gioKetThucHienTai <= gioBatDauMoi)
                    {
                        // Lịch hiện tại kết thúc trước lịch mới
                        var thoiGianNghiThucTe = gioBatDauMoi.ToTimeSpan() - gioKetThucHienTai.ToTimeSpan();
                        if (thoiGianNghiThucTe < thoiGianNghi)
                        {
                            invalidCodes.Add(hocSinhCode);
                            break;
                        }
                    }
                }

                if (invalidCodes.Contains(hocSinhCode))
                {
                    break; // Đã tìm thấy trùng lịch, không cần kiểm tra thêm lịch mới
                }
            }
        }
        string tatCaLoi = string.Join(", ", invalidCodes);
        if(invalidCodes.Count>0) throw new Exception($"Học sinh {tatCaLoi} bị trùng lịch");
        return invalidCodes.Count == 0;
    }
    private async Task<bool> ExistHocSinh(List<string> list, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        foreach (var item in list)
        {
            var exist = await _context.HocSinhs.AnyAsync(hs => hs.Code == item && hs.CoSoId == coSoId);
            if(!exist) return false;
        }
        return true;
    }

    private bool EnoughHocSinh(List<string> list)
    {
        return list.Count >= 3;
    }

    private async Task<bool> ChuaBatDau(EditLichHocCommand command, 
        List<LichHocDto> lichHocs, CancellationToken token)
    {
        var daBatDau = await _context.LichHocs
            .AnyAsync(lh => lh.NgayBatDau >= DateOnly.FromDateTime(DateTime.Now));
        var lichHocHienTais = await _context.LichHocs
                 .Where(lh => lh.TenLop == command.LopHocDto.TenLop)
                 .Select(lh => lh.Id)
                 .ToListAsync();
        if (daBatDau)
        {
            foreach (var lh in lichHocs)
            {
                if (lh.Id == null) return false;
                else
                    if (!lichHocHienTais.Contains((Guid)lh.Id)) return false;
            }
        }
        return true;
    }

    private async Task<bool> ValidLichHoc(EditLichHocCommand command, 
        List<LichHocDto> lichHocs, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        var lichHocHienTais = await _context.LichHocs
                 .Where(lh => lh.TenLop == command.LopHocDto.TenLop)
                 .Select(lh => lh.Id)
                 .ToListAsync();
            if (lichHocs == null || lichHocs.Count == 0)
            {
                return false; // Danh sách rỗng hoặc null thì không cần validate
            }
            var listThu = lichHocs.Select(lh => lh.Thu).Distinct()
                .ToList();
            if (listThu.Count != lichHocs.Count) return false;
            foreach (var lichHoc in lichHocs)
            {
                var checktime1 = TimeOnly.TryParse(lichHoc.GioBatDau, out var gioBatDau);
                var checktime2 = TimeOnly.TryParse(lichHoc.GioKetThuc, out var gioKetThuc);
                if (!checktime1 || !checktime2) return false;
                if (lichHoc.Thu < 2 || lichHoc.Thu > 8) return false;
                var check = await ExistPhong(lichHoc.PhongId, cToken);
                if (!check) return false;

                // Kiểm tra GioBatDau định dạng TimeOnly
                if (!BeValidTimeOnly(gioBatDau) || gioBatDau < new TimeOnly(8, 0) || gioBatDau > new TimeOnly(20, 0))
                {
                    return false;
                }

                // Kiểm tra GioKetThuc định dạng TimeOnly
                if (!BeValidTimeOnly(gioKetThuc) || gioKetThuc < new TimeOnly(10, 0) || gioKetThuc > new TimeOnly(22, 0))
                {
                    return false;
                }

                if (gioKetThuc < gioBatDau.AddHours(2))
                {
                    return false;
                }
            }
            var lichHocData = await _context.LichHocs
                .Where(lh=>!lichHocHienTais.Contains(lh.Id)&& lh.Phong.CoSoId == coSoId)
                .Select(l => new {
                    l.GiaoVienCode,
                    l.PhongId,
                    l.GioBatDau,
                    l.GioKetThuc,
                    l.Thu,
                    l.NgayBatDau,
                    l.NgayKetThuc
                })
                .ToListAsync();
            foreach (var lichHoc in lichHocs)
            {
                var checkThuAndPhong = lichHocData
                    .Any(lh => lh.Thu == lichHoc.Thu
                    && (lh.PhongId == lichHoc.PhongId || lh.GiaoVienCode == command.LopHocDto.GiaoVienCode)
                    && TinhNgayBuoiHocCuoiCung(lh.NgayKetThuc, lh.Thu) >= command.LopHocDto.NgayBatDau
                    && !(lh.GioKetThuc <= TimeOnly.Parse(lichHoc.GioBatDau).AddMinutes(-15)
                    || lh.GioBatDau >= TimeOnly.Parse(lichHoc.GioKetThuc).AddMinutes(15)));
                if (checkThuAndPhong) return false;
            }
        return true;
    }
    private bool BeValidTimeOnly(TimeOnly time)
    {
        return time != default; // Kiểm tra xem TimeOnly có giá trị mặc định hay không
    }
    private async Task<bool> ExistPhong(int phongId, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.Phongs.AnyAsync(p => p.Id == phongId && p.CoSoId == coSoId && p.TrangThai == "use");
    }
    // Phương thức tính toán ngày buổi học cuối cùng
    private DateOnly TinhNgayBuoiHocCuoiCung(DateOnly ngayKetThuc, int thu)
    {
        // Tính toán ngày buổi học cuối cùng dựa trên ngày kết thúc và thứ
        var ngayBuoiHocCuoiCung = ngayKetThuc;
        var ThutrongTuan = (thu < 8) ? thu - 1 : thu - 8;
        while (ngayBuoiHocCuoiCung.DayOfWeek != (DayOfWeek)ThutrongTuan)
        {
            ngayBuoiHocCuoiCung = ngayBuoiHocCuoiCung.AddDays(-1);
        }
        return ngayBuoiHocCuoiCung;
    }
    private async Task<bool> ExistClassName(EditLichHocCommand command, 
        string tenLop, CancellationToken cToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        return await _context.LichHocs.AnyAsync(l => l.TenLop == tenLop 
        && l.Phong.CoSoId == coSoId
        && l.NgayBatDau == command.LopHocDto.NgayBatDau
        && l.NgayKetThuc == command.LopHocDto.NgayKetThuc
        && l.GiaoVienCode == command.LopHocDto.GiaoVienCode
        && l.HocPhi == command.LopHocDto.HocPhi
        && l.ChuongTrinhId == command.LopHocDto.ChuongTrinhId
        && l.NgayKetThuc > DateOnly.FromDateTime(DateTime.Now), cToken);
    }
}

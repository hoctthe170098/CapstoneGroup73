using FluentValidation;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;

public class EditLichHocCommandValidator : AbstractValidator<EditLichHocCommand>
{
    public EditLichHocCommandValidator()
    {
        RuleFor(v => v.LichHocDto)
            .NotNull().WithMessage("Dữ liệu lịch học không được để trống.");

        RuleFor(v => v.LichHocDto.Id)
            .NotEmpty().WithMessage("ID không được để trống.");

        RuleFor(v => v.LichHocDto)
            .Must(dto =>
                dto.Thu.HasValue || dto.PhongId.HasValue ||
                !string.IsNullOrWhiteSpace(dto.TenLop) ||
                !string.IsNullOrWhiteSpace(dto.GioBatDau) ||
                !string.IsNullOrWhiteSpace(dto.GioKetThuc) ||
                dto.HocPhi.HasValue ||
                !string.IsNullOrWhiteSpace(dto.TrangThai) ||
                !string.IsNullOrWhiteSpace(dto.GiaoVienCode) ||
                dto.ChuongTrinhId.HasValue)
            .WithMessage("Ít nhất một trường cần được cập nhật.");

        RuleFor(v => v.LichHocDto.Thu)
            .InclusiveBetween(2, 8).When(v => v.LichHocDto.Thu.HasValue)
            .WithMessage("Thứ không hợp lệ, phải từ thứ 2 đến Chủ nhật.");

        RuleFor(v => v.LichHocDto.PhongId)
            .GreaterThan(0).When(v => v.LichHocDto.PhongId.HasValue)
            .WithMessage("Phòng ID không hợp lệ.");

        RuleFor(v => v.LichHocDto.TenLop)
            .MaximumLength(100).When(v => !string.IsNullOrWhiteSpace(v.LichHocDto.TenLop))
            .WithMessage("Tên lớp không được dài quá 100 ký tự.");

        RuleFor(v => v.LichHocDto.GioBatDau)
            .Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d$")
            .When(v => !string.IsNullOrWhiteSpace(v.LichHocDto.GioBatDau))
            .WithMessage("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");

        RuleFor(v => v.LichHocDto.GioKetThuc)
            .Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d$")
            .When(v => !string.IsNullOrWhiteSpace(v.LichHocDto.GioKetThuc))
            .WithMessage("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");

        RuleFor(v => v.LichHocDto)
            .Must(dto =>
            {
                if (!string.IsNullOrWhiteSpace(dto.GioBatDau) &&
                    !string.IsNullOrWhiteSpace(dto.GioKetThuc) &&
                    TimeOnly.TryParse(dto.GioBatDau, out var gioBatDau) &&
                    TimeOnly.TryParse(dto.GioKetThuc, out var gioKetThuc))
                {
                    return gioBatDau < gioKetThuc;
                }
                return true;
            })
            .WithMessage("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

        RuleFor(v => v.LichHocDto.NgayBatDau)
            .LessThanOrEqualTo(v => v.LichHocDto.NgayKetThuc)
            .WithMessage("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

        RuleFor(v => v.LichHocDto.HocPhi)
            .GreaterThanOrEqualTo(0).When(v => v.LichHocDto.HocPhi.HasValue)
            .WithMessage("Học phí không hợp lệ.");

        RuleFor(v => v.LichHocDto.TrangThai)
            .MaximumLength(50).When(v => !string.IsNullOrWhiteSpace(v.LichHocDto.TrangThai))
            .WithMessage("Trạng thái không được dài quá 50 ký tự.");

        RuleFor(v => v.LichHocDto.GiaoVienCode)
            .MaximumLength(50).When(v => !string.IsNullOrWhiteSpace(v.LichHocDto.GiaoVienCode))
            .WithMessage("Mã giáo viên không được dài quá 50 ký tự.");

        RuleFor(v => v.LichHocDto.ChuongTrinhId)
            .GreaterThan(0).When(v => v.LichHocDto.ChuongTrinhId.HasValue)
            .WithMessage("Chương trình ID không hợp lệ.");
    }
}

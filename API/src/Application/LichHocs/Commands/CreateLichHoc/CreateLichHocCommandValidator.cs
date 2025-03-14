namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;

public class CreateLichHocCommandValidator : AbstractValidator<CreateLichHocCommand>
{
    public CreateLichHocCommandValidator()
    {
        RuleFor(v => v.LichHocDto).NotNull().WithMessage("Dữ liệu lịch học không được để trống.");

        RuleFor(v => v.LichHocDto.TenLop)
            .NotEmpty().WithMessage("Tên lớp không được để trống.")
            .MaximumLength(100).WithMessage("Tên lớp không được dài quá 100 ký tự.");

        RuleFor(v => v.LichHocDto.TrangThai)
            .NotEmpty().WithMessage("Trạng thái không được để trống.");

        RuleFor(v => v.LichHocDto.GiaoVienCode)
            .NotEmpty().WithMessage("Mã giáo viên không được để trống.")
            .MaximumLength(50).WithMessage("Mã giáo viên không được dài quá 50 ký tự.");

        RuleFor(v => v.LichHocDto.Thu)
            .InclusiveBetween(2, 8).WithMessage("Thứ không hợp lệ, phải từ thứ 2 đến Chủ nhật.");

        RuleFor(v => v.LichHocDto.GioBatDau)
            .NotEmpty().WithMessage("Giờ bắt đầu không được để trống.")
            .Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d$").WithMessage("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");

        RuleFor(v => v.LichHocDto.GioKetThuc)
            .NotEmpty().WithMessage("Giờ kết thúc không được để trống.")
            .Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d$").WithMessage("Định dạng giờ không hợp lệ. Định dạng hợp lệ: HH:mm.");

        RuleFor(v => v.LichHocDto)
            .Must(dto =>
            {
                if (TimeOnly.TryParse(dto.GioBatDau, out var gioBatDau) &&
                    TimeOnly.TryParse(dto.GioKetThuc, out var gioKetThuc))
                {
                    return gioBatDau < gioKetThuc;
                }
                return false;
            }).WithMessage("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

        RuleFor(v => v.LichHocDto.NgayBatDau)
            .LessThanOrEqualTo(v => v.LichHocDto.NgayKetThuc)
            .WithMessage("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

        RuleFor(v => v.LichHocDto.HocPhi)
            .GreaterThanOrEqualTo(0).WithMessage("Học phí không hợp lệ.");

        RuleFor(v => v.LichHocDto.PhongId)
            .GreaterThan(0).WithMessage("Phòng ID không hợp lệ.");

        RuleFor(v => v.LichHocDto.ChuongTrinhId)
            .GreaterThan(0).WithMessage("Chương trình ID không hợp lệ.");
    }
}

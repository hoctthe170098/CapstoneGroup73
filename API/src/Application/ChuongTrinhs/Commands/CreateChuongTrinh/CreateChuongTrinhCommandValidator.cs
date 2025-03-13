using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
public class CreateChuongTrinhCommandValidator : AbstractValidator<CreateChuongTrinhCommand>
{
    public CreateChuongTrinhCommandValidator()
    {
        RuleFor(v => v.ChuongTrinhDto.TieuDe)
            .MaximumLength(30)
            .NotEmpty();

        RuleFor(v => v.ChuongTrinhDto.MoTa)
            .MaximumLength(100)
            .NotEmpty();

        When(v => v.ChuongTrinhDto.NoiDungBaiHocs != null && v.ChuongTrinhDto.NoiDungBaiHocs.Any(), () =>
        {
            RuleFor(v => v.ChuongTrinhDto.NoiDungBaiHocs)
                .Must(BeSequentialAndUnique)
                .WithMessage("Số thứ tự nội dung bài học phải bắt đầu từ 1 và tăng dần, không được trùng lặp.")
                .WithErrorCode("STT");

            RuleForEach(v => v.ChuongTrinhDto.NoiDungBaiHocs)
                .ChildRules(noiDung =>
                {
                    noiDung.RuleFor(n => n.TieuDe)
                        .MaximumLength(50)
                        .NotEmpty();

                    noiDung.RuleFor(n => n.Mota)
                        .MaximumLength(200)
                        .NotEmpty();

                    noiDung.RuleFor(n => n.SoThuTu)
                        .GreaterThan(0);

                    noiDung.RuleForEach(n => n.TaiLieuHocTaps)
                        .ChildRules(taiLieu =>
                        {
                            taiLieu.RuleFor(t => t.urlType)
                                .MaximumLength(20)
                                .NotEmpty();
                        });
                });
        });
    }
    private bool BeSequentialAndUnique(List<CreateNoiDungBaiHocDto>? noiDungBaiHocs)
    {
        if (noiDungBaiHocs == null || !noiDungBaiHocs.Any()) return true;

        var soThuTuDaDung = new HashSet<int>();
        int soThuTuDuKien = 1;

        foreach (var noiDung in noiDungBaiHocs)
        {
            if (noiDung.SoThuTu != soThuTuDuKien) return false;
            if (soThuTuDaDung.Contains(noiDung.SoThuTu)) return false;

            soThuTuDaDung.Add(noiDung.SoThuTu);
            soThuTuDuKien++;
        }
        return true;
    }
}

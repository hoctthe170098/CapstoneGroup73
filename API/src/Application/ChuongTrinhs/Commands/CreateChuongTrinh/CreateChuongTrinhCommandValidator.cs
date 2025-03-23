using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
public class CreateChuongTrinhCommandValidator : AbstractValidator<CreateChuongTrinhCommand>
{
    public CreateChuongTrinhCommandValidator()
    {
        RuleFor(v => v.ChuongTrinhDto.TieuDe)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(v => v.ChuongTrinhDto.MoTa)
            .NotEmpty()
            .MaximumLength(100);

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
                                .NotNull()
                                .NotEmpty()
                                .Must(FormatType)
                                .WithMessage("Định dạng file không hợp lệ.");
                            taiLieu.RuleFor(t => t.File)
                            .NotNull()
                            .Must(ValidLengthFile)
                            .WithMessage("Kích thước file không được vượt quá 10MB và tên của file không được vượt quá 200 ký tự");
                        });
                });
        });
    }

    private bool FormatType(string urlType)
    {
        if(urlType!="pdf"&&urlType!="word") return false;
        return true;
    }

    private bool ValidLengthFile(IFormFile file)
    {
        if(file==null) return false;
        if(file.Length==0) return false;
        long maxSizeInBytes = 10 * 1024 * 1024; // 10MB
        if (file.Length > maxSizeInBytes) return false;
        var ten = Path.GetFileNameWithoutExtension(file.FileName);
        if (ten.Length > 200) return false;
        return true;
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

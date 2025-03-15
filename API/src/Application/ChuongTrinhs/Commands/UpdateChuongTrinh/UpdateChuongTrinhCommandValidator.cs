using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
public class UpdateChuongTrinhCommandValidator : AbstractValidator<UpdateChuongTrinhCommand>
{
    public UpdateChuongTrinhCommandValidator()
    {
        RuleFor(v => v.ChuongTrinhDto.Id)
            .NotEmpty()
            .Must(BeValidInt)
            .WithMessage("Id chương trình phải là một số nguyên hợp lệ.")
            .WithErrorCode("Format");
        RuleFor(v => v.ChuongTrinhDto.TieuDe)
           .MaximumLength(30)
           .NotEmpty();

        RuleFor(v => v.ChuongTrinhDto.MoTa)
            .MaximumLength(100)
            .NotEmpty();
        RuleFor(v=>v.ChuongTrinhDto.TrangThai)
            .NotEmpty()
            .Must(BeValidStatus)
            .WithMessage("Trạng thái không hợp lệ.")
            .WithErrorCode("Format");
        When(v => v.ChuongTrinhDto.NoiDungBaiHocs != null && v.ChuongTrinhDto.NoiDungBaiHocs.Any(), () =>
        {
            RuleFor(v => v.ChuongTrinhDto.NoiDungBaiHocs)
            .NotEmpty()
            .Must(BeSequentialAndUnique)
            .WithMessage("Số thứ tự nội dung bài học phải bắt đầu từ 1 và tăng dần, không được trùng lặp.")
            .WithErrorCode("STT");
                RuleForEach(v => v.ChuongTrinhDto.NoiDungBaiHocs)
               .ChildRules(noiDung =>
               {
                   noiDung.RuleFor(n => n.Id)
                       .NotEmpty()
                       .Must(BeValidGuidOrNull)
                       .WithMessage("Id nội dung bài học phải là một Guid hợp lệ.")
                       .WithErrorCode("Format");
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
                           taiLieu.RuleFor(t => t.Id)
                               .Must(BeValidGuidOrNull)
                               .WithMessage("Id tài liệu học tập phải là một Guid hợp lệ hoặc null.")
                               .WithErrorCode("Format");
                           taiLieu.RuleFor(t => t.urlType)
                               .MaximumLength(20)
                               .NotEmpty();
                       });
               });         
        });      
    }
    private bool BeSequentialAndUnique(List<UpdateNoiDungBaiHocDto>? noiDungBaiHocs)
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
    private bool BeValidGuidOrNull(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return true;
        }
        return Guid.TryParse(id, out _);
    }
    private bool BeValidInt(int id)
    {
        return int.TryParse(id.ToString(), out _);
    }
    private bool BeValidStatus(string ? status)
    {
        if (status == "use" || status == "notuse") return true;
        return false;
    }
}

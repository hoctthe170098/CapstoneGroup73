using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
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
                   .Must(ValidListTaiLieu)
                   .WithMessage("Danh sách tài liệu không hợp lệ")
                       .ChildRules(taiLieu =>
                       {
                           taiLieu.RuleFor(t => t.Id)
                               .Must(BeValidGuidOrNull)
                               .WithMessage("Id tài liệu học tập phải là một Guid hợp lệ hoặc null.")
                               .WithErrorCode("Format");
                           taiLieu.RuleFor(t => t.urlType)
                                .MaximumLength(20)
                                .NotNull()
                                .NotEmpty()
                                .Must(FormatType)
                                .WithMessage("Định dạng file không hợp lệ.");
                           taiLieu.RuleFor(t => t.File)
                           .Must(ValidFile)
                           .WithMessage("Kích thước file không được vượt quá 10MB và tên của file không được vượt quá 200 ký tự");
                       });
               });         
        });      
    }

    private bool ValidListTaiLieu(UpdateTaiLieuHocTapDto taiLieu)
    {
        if(taiLieu.Id!=null&&taiLieu.File==null)return true;
        if(taiLieu.Id==null&&taiLieu.File!=null)return true;
        return false;
    }

    private bool FormatType(string urlType)
    {
        if (urlType != "pdf" && urlType != "word") return false;
        return true;
    }

    private bool ValidFile(IFormFile? file)
    {
        if (file != null)
        {
            if (file.Length == 0) return false;
            long maxSizeInBytes = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxSizeInBytes) return false;
            var ten = Path.GetFileNameWithoutExtension(file.FileName);
            if (ten.Length > 200) return false;
        }      
        return true;
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
}

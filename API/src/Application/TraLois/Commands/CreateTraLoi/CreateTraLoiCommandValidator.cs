using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Interfaces;

namespace StudyFlow.Application.TraLois.Commands.CreateTraLoi;

public class CreateTraLoiCommandValidator : AbstractValidator<CreateTraLoiCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTraLoiCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.NoiDung)
            .MaximumLength(750).WithMessage("Nội dung không được vượt quá 750 ký tự.");

        RuleFor(x => x.TepDinhKem)
            .Must(BeValidFile)
            .When(x => x.TepDinhKem != null)
            .WithMessage("Tệp đính kèm phải là .doc, .docx, hoặc .pdf và không được vượt quá 10MB.");

    }

    private bool BeValidFile(IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        bool isExtensionValid = allowedExtensions.Contains(extension);
        bool isSizeValid = file.Length <= 10 * 1024 * 1024;

        return isExtensionValid && isSizeValid;
    }


}

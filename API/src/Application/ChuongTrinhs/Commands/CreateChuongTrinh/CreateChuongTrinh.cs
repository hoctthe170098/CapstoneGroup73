using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;

public record CreateChuongTrinhCommand : IRequest<Output>
{
    public required string TieuDe { get; init; }
    public required string MoTa { get; init; }
}

public class CreateChuongTrinhCommandHandler : IRequestHandler<CreateChuongTrinhCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public CreateChuongTrinhCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(CreateChuongTrinhCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TieuDe) || string.IsNullOrWhiteSpace(request.MoTa))
            throw new NotFoundDataException("Dữ liệu không được để trống");

        if (request.TieuDe.Length > 100 || request.MoTa.Length > 500)
            throw new WrongInputException("Độ dài tiêu đề hoặc mô tả không hợp lệ!");

        var chuongTrinh = new ChuongTrinh
        {
            TieuDe = request.TieuDe,
            MoTa = request.MoTa
        };

        _context.ChuongTrinhs.Add(chuongTrinh);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = chuongTrinh,
            code = 200,
            message = "Tạo chương trình mới thành công"
        };
    }
}

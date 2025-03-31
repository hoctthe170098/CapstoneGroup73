using StudyFlow.Application.ChinhSachs.Commands.CreateChinhSach;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChinhSaches.Commands.CreateChinhSach;

public record CreateChinhSachCommand : IRequest<Output>
{
    public required CreateChinhSachDto CreateChinhSachDto { get; init; }
}

public class CreateChinhSachCommandHandler : IRequestHandler<CreateChinhSachCommand, Output>
{
    private readonly IApplicationDbContext _context;
    public CreateChinhSachCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(CreateChinhSachCommand request, CancellationToken cancellationToken)
    {
        var existingChinhSach = await _context.ChinhSaches
            .AnyAsync(cs => cs.Ten == request.CreateChinhSachDto.Ten, cancellationToken);

        if (existingChinhSach)
            throw new WrongInputException("Tên chính sách này đã tồn tại. Vui lòng chọn tên khác cho chính sách này.");

        var chinhSach = new ChinhSach
        {
            Ten = request.CreateChinhSachDto.Ten,
            Mota = request.CreateChinhSachDto.Mota,
            PhanTramGiam = request.CreateChinhSachDto.PhanTramGiam
        };

        _context.ChinhSaches.Add(chinhSach);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = chinhSach,
            code = 200,
            message = "Thêm chính sách thành công."
        };
    }
}

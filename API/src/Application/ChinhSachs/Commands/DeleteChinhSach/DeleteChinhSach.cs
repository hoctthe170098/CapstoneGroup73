using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ChinhSachs.Commands.DeleteChinhSach;

public record DeleteChinhSachCommand(int Id) : IRequest<Output>;

public class DeleteChinhSachCommandHandler : IRequestHandler<DeleteChinhSachCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public DeleteChinhSachCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(DeleteChinhSachCommand request, CancellationToken cancellationToken)
    {
        var chinhSach = await _context.ChinhSaches
            .Include(cs => cs.HocSinhs)
            .FirstOrDefaultAsync(cs => cs.Id == request.Id, cancellationToken);

        if (chinhSach == null)
        {
            throw new NotFoundDataException($"Không tìm thấy chính sách với ID {request.Id} này.");
        }

        if (chinhSach.HocSinhs.Any())
        {
            throw new WrongInputException("Chính sách học phí đang được áp dụng cho học viên. Vui lòng cập nhật hồ sơ trước khi xóa.");
        }

        _context.ChinhSaches.Remove(chinhSach);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = null,
            code = 200,
            message = "Xóa chính sách thành công."
        };
    }
}

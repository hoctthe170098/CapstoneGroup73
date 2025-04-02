using MediatR;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using System;

namespace StudyFlow.Application.BaiTaps.Commands.DeleteBaiTap;

public record DeleteBaiTapCommand(Guid Id) : IRequest<Output>;
public class DeleteBaiTapCommandHandler : IRequestHandler<DeleteBaiTapCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public DeleteBaiTapCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(DeleteBaiTapCommand request, CancellationToken cancellationToken)
    {
        var baiTap = await _context.BaiTaps
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (baiTap == null)
        {
            throw new NotFoundDataException("Không tìm thấy bài tập.");
        }

        _context.BaiTaps.Remove(baiTap);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            code = 200,
            data = null,
            message = "Xóa bài tập thành công."
        };
    }
}

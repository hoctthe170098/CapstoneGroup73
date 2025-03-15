using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChinhSachs.Commands.UpdateChinhSach;

public record UpdateChinhSachCommand : IRequest<Output>
{
    public required UpdateChinhSachDto UpdateChinhSachDto { get; init; }
}

public class UpdateChinhSachCommandHandler : IRequestHandler<UpdateChinhSachCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public UpdateChinhSachCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(UpdateChinhSachCommand request, CancellationToken cancellationToken)
    {
        var chinhSach = await _context.ChinhSaches.FindAsync(request.UpdateChinhSachDto.Id);

        if (chinhSach == null)
        {
            throw new NotFoundDataException($"Không tìm thấy chính sách với ID {request.UpdateChinhSachDto.Id} này.");
        }

        if (!string.IsNullOrWhiteSpace(request.UpdateChinhSachDto.Ten))
        {
            var existingChinhSach = await _context.ChinhSaches
                .AnyAsync(cs => cs.Ten == request.UpdateChinhSachDto.Ten && cs.Id != request.UpdateChinhSachDto.Id, cancellationToken);

            if (existingChinhSach)
                throw new WrongInputException("Tên chính sách đã tồn tại. Vui lòng chọn tên khác.");

            chinhSach.Ten = request.UpdateChinhSachDto.Ten;
        }

        if (!string.IsNullOrWhiteSpace(request.UpdateChinhSachDto.Mota))
            chinhSach.Mota = request.UpdateChinhSachDto.Mota;

        if (request.UpdateChinhSachDto.PhanTramGiam.HasValue)
        {
            if (request.UpdateChinhSachDto.PhanTramGiam.Value <= 0f || request.UpdateChinhSachDto.PhanTramGiam.Value >= 0.1f)
            {
                throw new WrongInputException("Phần trăm giảm phải nằm trong khoảng (0,0.1).");
            }
            chinhSach.PhanTramGiam = request.UpdateChinhSachDto.PhanTramGiam.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = chinhSach,
            code = 200,
            message = "Cập nhật chính sách thành công."
        };
    }
}

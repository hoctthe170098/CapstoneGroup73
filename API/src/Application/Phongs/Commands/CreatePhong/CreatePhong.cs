using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Phongs.Commands.CreatePhong;

public record CreatePhongCommand : IRequest<Output>
{
    public required string Ten { get; init; }
    public required Guid CoSoId { get; init; }
}

public class CreatePhongCommandHandler : IRequestHandler<CreatePhongCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public CreatePhongCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(CreatePhongCommand request, CancellationToken cancellationToken)
    {
        // Validate đầu vào
        ValidateRequest(request);

        // Kiểm tra CoSo tồn tại
        var coSoExists = await _context.CoSos.AnyAsync(cs => cs.Id == request.CoSoId, cancellationToken);
        if (!coSoExists)
            throw new NotFoundDataException("Không tìm thấy cơ sở.");

        // Check trùng tên phòng trong cơ sở
        var isPhongExisted = await _context.Phongs
            .AnyAsync(p => p.Ten.Trim().ToLower() == request.Ten.Trim().ToLower() && p.CoSoId == request.CoSoId, cancellationToken);

        if (isPhongExisted)
            throw new Exception($"Tên phòng '{request.Ten}' đã tồn tại trong cơ sở này.");

        // Tạo mới phòng với trạng thái mặc định là 'use'
        var phong = new Phong
        {
            Ten = request.Ten.Trim(),
            TrangThai = "use", // Trạng thái mặc định
            CoSoId = request.CoSoId
        };

        _context.Phongs.Add(phong);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = phong,
            code = 200,
            message = "Tạo phòng thành công"
        };
    }

    private void ValidateRequest(CreatePhongCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Ten))
            throw new WrongInputException("Tên phòng không được để trống!");

        if (request.Ten.Length > 50)
            throw new WrongInputException("Tên phòng không được dài quá 50 ký tự!");

        if (request.CoSoId == Guid.Empty)
            throw new WrongInputException("CoSoId không hợp lệ!");
    }
}

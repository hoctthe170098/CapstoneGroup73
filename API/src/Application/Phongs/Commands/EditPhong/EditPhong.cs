using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Phongs.Commands.EditPhong;

public record EditPhongCommand : IRequest<Output>
{
    public required int Id { get; init; }
    public required string Ten { get; init; }
    public required string TrangThai { get; init; }
    public required Guid CoSoId { get; init; }
}

public class EditPhongCommandHandler : IRequestHandler<EditPhongCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public EditPhongCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(EditPhongCommand request, CancellationToken cancellationToken)
    {
        // Validate dữ liệu đầu vào
        ValidateRequest(request);

        // Lấy thông tin phòng cần cập nhật
        var phong = await _context.Phongs
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (phong == null)
            throw new NotFoundDataException($"Không tìm thấy phòng với Id = {request.Id}");

        // Kiểm tra cơ sở có tồn tại không
        var coSoExists = await _context.CoSos.AnyAsync(cs => cs.Id == request.CoSoId, cancellationToken);
        if (!coSoExists)
            throw new NotFoundDataException("Không tìm thấy cơ sở ID.");

        // Kiểm tra trùng tên phòng trong cùng cơ sở, bỏ qua chính nó
        var isDuplicateName = await _context.Phongs
            .AnyAsync(p => p.Id != request.Id &&
                           p.CoSoId == request.CoSoId &&
                           p.Ten.Trim().ToLower() == request.Ten.Trim().ToLower(), cancellationToken);

        if (isDuplicateName)
            throw new Exception($"Tên phòng '{request.Ten}' đã tồn tại trong cơ sở này.");

        // Cập nhật thông tin
        phong.Ten = request.Ten.Trim();
        phong.TrangThai = request.TrangThai.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = phong,
            code = 200,
            message = "Cập nhật phòng thành công"
        };
    }

    private void ValidateRequest(EditPhongCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Ten))
            throw new WrongInputException("Tên phòng không được để trống!");

        if (request.Ten.Length > 50)
            throw new WrongInputException("Tên phòng không được dài quá 50 ký tự!");

        if (string.IsNullOrWhiteSpace(request.TrangThai))
            throw new WrongInputException("Trạng thái không được để trống!");

        if (request.TrangThai != "use" && request.TrangThai != "notuse")
            throw new WrongInputException("Trạng thái chỉ được là 'use' hoặc 'notuse'!");

        if (request.CoSoId == Guid.Empty)
            throw new WrongInputException("CoSoId không hợp lệ!");
    }
}

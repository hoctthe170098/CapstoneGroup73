using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Slots.Commands;

public record CreateSlotCommand : IRequest<Output>
{
    public string? Ten { get; set; }
    public TimeOnly? BatDau { get; set; }
    public TimeOnly? KetThuc { get; set; }
}

public class CreateSlotCommandHandler : IRequestHandler<CreateSlotCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public CreateSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(CreateSlotCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Ten) || request.BatDau == null || request.KetThuc == null)
        {
            throw new NotFoundDataException("Dữ liệu không được để trống.");
        }
        if (request.Ten.Length > 50)
        {
            throw new WrongInputException("Tên slot không được quá 50 ký tự.");
        }
        if (request.BatDau >= request.KetThuc)
        {
            throw new WrongInputException("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");
        }
        var slot = new Slot
        {
            Ten = request.Ten,
            BatDau = request.BatDau.Value,
            KetThuc = request.KetThuc.Value
        };

        _context.Slots.Add(slot);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = slot,
            code = 200,
            message = "Tạo slot mới thành công"
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Commands.CreateCoSo;
public record CreateCoSoComand : IRequest<Output>
{
    public  string? Ten { get; init; }
    public  string? DiaChi { get; init; }
    public  string? SoDienThoai { get; init; }
}
public class CreateCoSoComandHandler : IRequestHandler<CreateCoSoComand, Output>
{
    private readonly IApplicationDbContext _context;
    public CreateCoSoComandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Output> Handle(CreateCoSoComand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Ten) 
            ||string.IsNullOrWhiteSpace(request.DiaChi) 
            ||string.IsNullOrWhiteSpace(request.SoDienThoai))
            throw new NotFoundDataException("Dữ liệu không được để trống");
        if (request.Ten.Length > 30 || request.DiaChi.Length > 200
            || request.SoDienThoai.Length > 11) throw new WrongInputException("Độ dài dữ liệu không hợp lệ!");
        if (!request.SoDienThoai.StartsWith("0") || !request.SoDienThoai.All(char.IsDigit))
        {
            throw new FormatException("Số điện thoại nhập không hợp lệ");
        }
        var coso = new CoSo
        {
            Id = new Guid(),
            Ten = request.Ten,
            DiaChi = request.DiaChi,
            TrangThai = "open",
            SoDienThoai = request.SoDienThoai,
            Default = false
        };
        _context.CoSos.Add(coso);
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = coso,
            code = 200,
            message = "Tạo cơ sở mới thành công"
        };
    }
}

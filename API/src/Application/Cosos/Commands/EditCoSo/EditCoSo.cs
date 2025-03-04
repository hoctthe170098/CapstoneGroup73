using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Commands.EditCoSo;

public record EditCoSoComand : IRequest<Output>
{
    public string? Id { get; init; }
    public string? Ten { get; init; }
    public string? DiaChi { get; init; }
    public string? SoDienThoai { get; init; }
    public string? TrangThai { get; init; }
}
public class EditCoSoComandHandler : IRequestHandler<EditCoSoComand, Output>
{
    private readonly IApplicationDbContext _context;
    public EditCoSoComandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Output> Handle(EditCoSoComand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id)
           || string.IsNullOrWhiteSpace(request.Ten)
           || string.IsNullOrWhiteSpace(request.DiaChi)
           || string.IsNullOrWhiteSpace(request.SoDienThoai)
           || string.IsNullOrWhiteSpace(request.TrangThai))
            throw new NotFoundDataException("Dữ liệu không được để trống");
        if (request.Ten.Length > 30 || request.DiaChi.Length > 50
            || request.SoDienThoai.Length > 11
            ||(request.TrangThai!="close"&&request.TrangThai!="open")) 
            throw new WrongInputException("Độ dài dữ liệu không hợp lệ!");
        if (!request.SoDienThoai.StartsWith("0") 
            || !request.SoDienThoai.All(char.IsDigit))
        {
            throw new FormatException("Số điện thoại nhập không hợp lệ");
        }
        Guid g_id = Guid.Parse(request.Id);
        var coso = await _context.CoSos.FindAsync(new object[] { g_id }, cancellationToken);
        if (coso == null) throw new NotFoundIDException();
        else
        {
            coso.Ten = request.Ten;
            coso.DiaChi = request.DiaChi;
            coso.SoDienThoai = request.SoDienThoai;
            coso.TrangThai = request.TrangThai;           
        }
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = coso,
            code = 200,
            message = "Chỉnh sửa cơ sở thành công"
        };
    }
}

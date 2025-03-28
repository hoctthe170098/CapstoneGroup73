using CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ChinhSachs.Commands.DeleteBaiKiemTra;

public record DeleteBaiKiemTraCommand(Guid Id) : IRequest<Output>;

public class DeleteBaiKiemTraCommandHandler : IRequestHandler<DeleteBaiKiemTraCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<DeleteBaiKiemTraCommandHandler> _logger;

    public DeleteBaiKiemTraCommandHandler(IApplicationDbContext context
        , IWebHostEnvironment webHostEnvironment
        , ILogger<DeleteBaiKiemTraCommandHandler> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<Output> Handle(DeleteBaiKiemTraCommand request, CancellationToken cancellationToken)
    {
        var baiKiemTra = await _context.BaiKiemTras
            .FirstOrDefaultAsync(b=>b.Id == request.Id, cancellationToken);
        if (baiKiemTra == null)
        {
            throw new NotFoundDataException($"Không tìm thấy chính sách với ID {request.Id} này.");
        }
        if(baiKiemTra.TrangThai=="Đã kiểm tra")
        {
            return new Output
            {
                isError = true,
                data = null,
                code = 200,
                message = "Bài kiểm tra này đã kiểm tra, không thể xoá."
            };
        }
        DeleteFile(baiKiemTra.UrlFile);
        _context.BaiKiemTras.Remove(baiKiemTra);
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            data = null,
            code = 200,
            message = "Xóa bài kiểm tra thành công."
        };
    }
    private void DeleteFile(string filePath)
    {
        try
        {
            var fullFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
        }
        catch (IOException ioException)
        {
            _logger.LogError(ioException, "Error deleting file {FilePath}", filePath);
            throw new Exception($"Error deleting file {filePath}: {ioException.Message}");
        }
    }
}

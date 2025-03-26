
using Microsoft.AspNetCore.Hosting;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using Microsoft.Extensions.Logging;
using StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
namespace CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
public record DeleteChuongTrinhComand(int id) : IRequest<Output>;
public class DeleteChuongTrinhComandHandler : IRequestHandler<DeleteChuongTrinhComand,Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<DeleteChuongTrinhComandHandler> _logger;
    public DeleteChuongTrinhComandHandler(IApplicationDbContext context
        , IWebHostEnvironment webHostEnvironment
        , ILogger<DeleteChuongTrinhComandHandler> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }
    async Task<Output> IRequestHandler<DeleteChuongTrinhComand, Output>.Handle(DeleteChuongTrinhComand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ChuongTrinhs
            .Where(l => l.Id == request.id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundIDException();
        }
        var lop = await _context.LichHocs
            .AnyAsync(l => l.ChuongTrinhId == request.id);
        if (!lop)
        {
            var noidungbaiHocs = await _context.NoiDungBaiHocs
                .Where(n=>n.ChuongTrinhId==request.id).ToListAsync();
            foreach(var noidungbaihoc in noidungbaiHocs)
            {
                var tailieuHocTaps = await _context.TaiLieuHocTaps
                .Where(n => n.NoiDungBaiHocId == noidungbaihoc.Id).ToListAsync();
                foreach(var tailieuHocTap in tailieuHocTaps)
                {
                    DeleteFile(tailieuHocTap.urlFile);
                }
                _context.TaiLieuHocTaps.RemoveRange(tailieuHocTaps);
                await _context.SaveChangesAsync(cancellationToken);
            }
            _context.NoiDungBaiHocs.RemoveRange(noidungbaiHocs);
            await _context.SaveChangesAsync(cancellationToken);
            _context.ChuongTrinhs.Remove(entity);
        }
        else
        {
            entity.TrangThai = "notuse";
        }
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            code=200,
            data = entity,
            message = "Xoá chương trình thành công"
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

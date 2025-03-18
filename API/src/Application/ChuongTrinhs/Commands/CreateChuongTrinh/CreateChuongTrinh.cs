using Microsoft.AspNetCore.Hosting;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Ardalis.GuardClauses;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;

public record CreateChuongTrinhCommand : IRequest<Output>
{
    public required CreateChuongTrinhDto ChuongTrinhDto { get; init; }
}

public class CreateChuongTrinhCommandHandler : IRequestHandler<CreateChuongTrinhCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _uploadFolderPath;
    private readonly ILogger<CreateChuongTrinhCommandHandler> _logger;

    public CreateChuongTrinhCommandHandler(IApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<CreateChuongTrinhCommandHandler> logger)
    {
        _context = Guard.Against.Null(context);
        _webHostEnvironment = Guard.Against.Null(webHostEnvironment);
        _logger = Guard.Against.Null(logger);
        _uploadFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads");
        if (!Directory.Exists(_uploadFolderPath))
        {
            Directory.CreateDirectory(_uploadFolderPath);
        }
    }

    public async Task<Output> Handle(CreateChuongTrinhCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var chuongTrinhDto = request.ChuongTrinhDto;
            var chuongTrinh = new ChuongTrinh
            {
                TieuDe = chuongTrinhDto.TieuDe,
                MoTa = chuongTrinhDto.MoTa,
                TrangThai = "use"
            };
            _context.ChuongTrinhs.Add(chuongTrinh);
            await _context.SaveChangesAsync(cancellationToken);
            if (chuongTrinhDto.NoiDungBaiHocs != null || chuongTrinhDto.NoiDungBaiHocs?.Count > 0)
            {
                foreach (var noiDungDto in chuongTrinhDto.NoiDungBaiHocs)
                {
                    var noiDung = new NoiDungBaiHoc
                    {
                        Id = Guid.NewGuid(),
                        TieuDe = noiDungDto.TieuDe,
                        Mota = noiDungDto.Mota,
                        SoThuTu = noiDungDto.SoThuTu,
                        ChuongTrinhId = chuongTrinh.Id
                    };
                    _context.NoiDungBaiHocs.Add(noiDung);
                    if (noiDungDto.TaiLieuHocTaps != null && noiDungDto.TaiLieuHocTaps.Any())
                    {
                        foreach (var taiLieuDto in noiDungDto.TaiLieuHocTaps)
                        {
                            var taiLieu = new TaiLieuHocTap
                            {
                                Id = Guid.NewGuid(),
                                Ten = "",
                                urlType = taiLieuDto.urlType,
                                NgayTao = DateOnly.FromDateTime(DateTime.Now),
                                NoiDungBaiHocId = noiDung.Id,
                                urlFile = ""
                            };
                            if ((taiLieuDto.urlType == "pdf" || taiLieuDto.urlType == "video"
                                || taiLieuDto.urlType == "mp4"
                                ) && taiLieuDto.File != null && taiLieuDto.File.Length > 0)
                            {
                                try
                                {
                                    taiLieu.Ten = Path.GetFileNameWithoutExtension(taiLieuDto.File.FileName);
                                    if (taiLieu.Ten.Length > 200) 
                                        throw new FormatException("Tên của tài liệu không được vượt quá 200 ký tự");
                                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(taiLieuDto.File.FileName);
                                    var filePath = Path.Combine(_uploadFolderPath, fileName);
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await taiLieuDto.File.CopyToAsync(stream, cancellationToken);
                                    }
                                    taiLieu.urlFile = "/uploads/" + fileName;
                                }
                                catch (IOException ioException)
                                {
                                    _logger.LogError(ioException, "Error uploading file {FileName}", taiLieuDto.File.FileName);
                                    throw new Exception($"Error uploading file {taiLieuDto.File.FileName}: {ioException.Message}");
                                }
                            }
                            else throw new FormatException();
                            _context.TaiLieuHocTaps.Add(taiLieu);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }
            }      
            await _context.SaveChangesAsync(cancellationToken);
            return new Output
            {
                isError = false,
                data = chuongTrinh,
                code = 200,
                message = "Tạo chương trình mới thành công"
            };
        }
        catch
        {
            throw new WrongInputException();
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;

public record UpdateChuongTrinhCommand : IRequest<Output>
{
    public required UpdateChuongTrinhDto ChuongTrinhDto { get; init; }
}

public class UpdateChuongTrinhCommandHandler : IRequestHandler<UpdateChuongTrinhCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _uploadFolderPath;
    private readonly ILogger<UpdateChuongTrinhCommandHandler> _logger;

    public UpdateChuongTrinhCommandHandler(IApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<UpdateChuongTrinhCommandHandler> logger)
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
    public async Task<Output> Handle(UpdateChuongTrinhCommand request, CancellationToken cancellationToken)
    {
        var chuongTrinhDto = request.ChuongTrinhDto;
        var chuongTrinh = await _context.ChuongTrinhs
            .FindAsync(new object[] { chuongTrinhDto.Id }, cancellationToken);

        if (chuongTrinh == null)
        {
            throw new NotFoundIDException();
        }
        // 1. Kiểm tra NộiDungBaiHoc không tồn tại
        if (chuongTrinhDto.NoiDungBaiHocs != null)
        {
            foreach (var noiDungDto in chuongTrinhDto.NoiDungBaiHocs)
            {
                if (!string.IsNullOrEmpty(noiDungDto.Id))
                {
                    var noiDung = await _context.NoiDungBaiHocs
                        .FindAsync(new object[] { noiDungDto.Id }, cancellationToken);
                    if (noiDung == null || noiDung.ChuongTrinhId != chuongTrinhDto.Id)
                    {
                        throw new NotFoundIDException();
                    }
                    // Kiểm tra TaiLieuHocTap không tồn tại
                    if (noiDungDto.TaiLieuHocTaps != null)
                    {
                        foreach (var taiLieuDto in noiDungDto.TaiLieuHocTaps)
                        {
                            if (!string.IsNullOrEmpty(taiLieuDto.Id))
                            {
                                var taiLieu = await _context.TaiLieuHocTaps
                                    .FindAsync(new object[] { taiLieuDto.Id }, cancellationToken);
                                if (taiLieu == null 
                                    || taiLieu.NoiDungBaiHocId != Guid.Parse(noiDungDto.Id))
                                {
                                    throw new NotFoundIDException();
                                }
                            }
                        }
                    }
                }
            }
        }
        // 2. Xóa NộiDungBaiHoc không có trong danh sách cập nhật
        var noiDungHienTai = await _context.NoiDungBaiHocs
            .Where(n => n.ChuongTrinhId == chuongTrinhDto.Id)
            .ToListAsync(cancellationToken);

        if (chuongTrinhDto.NoiDungBaiHocs != null)
        {
            var noiDungIdsCapNhat = chuongTrinhDto.NoiDungBaiHocs
                .Where(n => !string.IsNullOrEmpty(n.Id))
                .Select(n => Guid.Parse(n.Id!))
                .ToList();
            var noiDungCanXoas = noiDungHienTai
                .Where(n => !noiDungIdsCapNhat.Contains(n.Id)).ToList();
            foreach(var noidungCanXoa in noiDungCanXoas)
            {
                var tailieucanxoas = await _context.TaiLieuHocTaps
                    .Where(n=>n.NoiDungBaiHocId==noidungCanXoa.Id)
                    .ToListAsync(cancellationToken);
                foreach(var tailieucanxoa in tailieucanxoas)
                {
                    DeleteFile(tailieucanxoa.urlFile);
                }
                _context.TaiLieuHocTaps.RemoveRange(tailieucanxoas);
            }
            _context.NoiDungBaiHocs.RemoveRange(noiDungCanXoas);
        }
        else
        {
            _context.NoiDungBaiHocs.RemoveRange(noiDungHienTai);
        }

        // 3. Thêm/Cập nhật NộiDungBaiHoc
        if (chuongTrinhDto.NoiDungBaiHocs != null)
        {
            foreach (var noiDungDto in chuongTrinhDto.NoiDungBaiHocs)
            {
                if (string.IsNullOrEmpty(noiDungDto.Id))
                {
                    // Thêm mới NộiDungBaiHoc
                    var noiDung = new NoiDungBaiHoc
                    {
                        Id = Guid.NewGuid(),
                        TieuDe = noiDungDto.TieuDe,
                        Mota = noiDungDto.Mota,
                        SoThuTu = noiDungDto.SoThuTu,
                        ChuongTrinhId = chuongTrinhDto.Id
                    };
                    _context.NoiDungBaiHocs.Add(noiDung);

                    // Thêm mới TaiLieuHocTap
                    if (noiDungDto.TaiLieuHocTaps != null)
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
                            await UploadFile(taiLieu, taiLieuDto,cancellationToken);
                            _context.TaiLieuHocTaps.Add(taiLieu);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }
                else
                {
                    // Cập nhật NộiDungBaiHoc
                    var noiDung = await _context.NoiDungBaiHocs.FindAsync(Guid.Parse(noiDungDto.Id));
                    if (noiDung != null)
                    {
                        noiDung.TieuDe = noiDungDto.TieuDe;
                        noiDung.Mota = noiDungDto.Mota;
                        noiDung.SoThuTu = noiDungDto.SoThuTu;
                        _context.NoiDungBaiHocs.Update(noiDung);
                        var TaiLieuHienTai = await _context.TaiLieuHocTaps
                           .Where(n => n.NoiDungBaiHocId == Guid.Parse(noiDungDto.Id))
                           .ToListAsync(cancellationToken);
                        // Cập nhật TaiLieuHocTap
                        if (noiDungDto.TaiLieuHocTaps != null)
                        {
                            //xoa TaiLieuHocTap
                            var TaiLieuIdsCapNhat = noiDungDto.TaiLieuHocTaps
                                .Where(n => !string.IsNullOrEmpty(n.Id))
                                .Select(n => Guid.Parse(n.Id!))
                                .ToList();
                            var TaiLieuCanXoas = TaiLieuHienTai
                                .Where(n => !TaiLieuIdsCapNhat.Contains(n.Id)).ToList();
                            _context.TaiLieuHocTaps.RemoveRange(TaiLieuCanXoas);
                            foreach (var taiLieuDto in noiDungDto.TaiLieuHocTaps)
                            {
                                if (string.IsNullOrEmpty(taiLieuDto.Id))
                                {
                                    // Thêm mới TaiLieuHocTap
                                    var taiLieu = new TaiLieuHocTap
                                    {
                                        Id = Guid.NewGuid(),
                                        Ten = "",
                                        urlType = taiLieuDto.urlType,
                                        NgayTao = DateOnly.FromDateTime(DateTime.Now),
                                        NoiDungBaiHocId = noiDung.Id,
                                        urlFile = ""
                                    };
                                    await UploadFile(taiLieu, taiLieuDto, cancellationToken);
                                    _context.TaiLieuHocTaps.Add(taiLieu);
                                    await _context.SaveChangesAsync(cancellationToken);
                                }
                            }
                        }
                    }
                }
            }
        }
        // Cập nhật thông tin ChuongTrinh
        chuongTrinh.TieuDe = chuongTrinhDto.TieuDe;
        chuongTrinh.MoTa = chuongTrinhDto.MoTa;
        _context.ChuongTrinhs.Update(chuongTrinh);
        await _context.SaveChangesAsync(cancellationToken);
        return new Output
        {
            isError = false,
            code = 200,
            data = chuongTrinh,
            message = "Cập nhật chương trình thành công"
        };
    }
    private async Task UploadFile(TaiLieuHocTap taiLieu, UpdateTaiLieuHocTapDto taiLieuDto, CancellationToken cancellationToken)
    {
        if ((taiLieuDto.urlType == "pdf" 
            || taiLieuDto.urlType == "video"
            || taiLieuDto.urlType == "mp4") 
            && taiLieuDto.File != null && taiLieuDto.File.Length > 0)
        {
            try
            {
                taiLieu.Ten = Path.GetFileNameWithoutExtension(taiLieuDto.File.FileName);
                if (taiLieu.Ten.Length > 50) throw new FormatException();
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

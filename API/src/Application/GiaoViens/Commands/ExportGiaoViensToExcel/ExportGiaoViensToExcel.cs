namespace StudyFlow.Application.GiaoViens.Commands.ExportGiaoViensToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination.GetGiaoViensWithPagination;

public record ExportGiaoViensToExcelCommand(string FolderPath, string FileName) : IRequest<FileContentResult>
{
}

public class ExportGiaoVienCommandHandler : IRequestHandler<ExportGiaoViensToExcelCommand, FileContentResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public ExportGiaoVienCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<FileContentResult> Handle(ExportGiaoViensToExcelCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra và xử lý thông tin folder
        string folderPath = string.IsNullOrWhiteSpace(request.FolderPath)
            ? Path.Combine(Directory.GetCurrentDirectory(), "ExportedFiles") // Mặc định nếu không truyền folder
            : request.FolderPath;

        // Kiểm tra nếu thư mục không tồn tại, tạo mới
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Kiểm tra tên file, nếu không truyền sẽ đặt tên mặc định
        string fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? "DanhSachGiaoVien.xlsx"
            : request.FileName.EndsWith(".xlsx") ? request.FileName : request.FileName + ".xlsx";

        string filePath = Path.Combine(folderPath, fileName);

        // Lấy danh sách giáo viên
        var giaoViens = await _context.GiaoViens
            .Include(gv => gv.Coso)
            .Include(gv => gv.LicHocs)
            .ToListAsync(cancellationToken);

        if (!giaoViens.Any())
        {
            throw new NotFoundDataException("Không có giáo viên nào để xuất.");
        }

        // Tạo danh sách DTO và lấy trạng thái từ Identity
        var giaoVienDtos = new List<GiaoVienDto>();
        foreach (var gv in giaoViens)
        {
            bool isActive = gv.UserId != null && await _identityService.IsUserActiveAsync(gv.UserId);

            giaoVienDtos.Add(new GiaoVienDto
            {
                Code = gv.Code,
                Ten = gv.Ten,
                GioiTinh = gv.GioiTinh,
                NgaySinh = gv.NgaySinh,
                Email = gv.Email,
                SoDienThoai = gv.SoDienThoai,
                DiaChi = gv.DiaChi,
                TruongDangDay = gv.TruongDangDay,
                TenCoSo = gv.Coso.Ten,
                TenLops = gv.LicHocs.Select(lh => lh.TenLop).Distinct().ToList(),
                IsActive = isActive
            });
        }

        // Tạo file Excel
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("DanhSachGiaoVien");

        var headers = new[] { "Mã GV", "Tên", "Giới Tính", "Ngày Sinh", "Email", "Số Điện Thoại", "Địa Chỉ", "Trường Đang Dạy", "Tên Cơ Sở", "Danh Sách Lớp", "Trạng Thái" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        for (int i = 0; i < giaoVienDtos.Count; i++)
        {
            var gv = giaoVienDtos[i];
            worksheet.Cells[i + 2, 1].Value = gv.Code;
            worksheet.Cells[i + 2, 2].Value = gv.Ten;
            worksheet.Cells[i + 2, 3].Value = gv.GioiTinh;
            worksheet.Cells[i + 2, 4].Value = gv.NgaySinh.ToString("yyyy-MM-dd");
            worksheet.Cells[i + 2, 5].Value = gv.Email;
            worksheet.Cells[i + 2, 6].Value = gv.SoDienThoai;
            worksheet.Cells[i + 2, 7].Value = gv.DiaChi;
            worksheet.Cells[i + 2, 8].Value = gv.TruongDangDay;
            worksheet.Cells[i + 2, 9].Value = gv.TenCoSo;
            worksheet.Cells[i + 2, 10].Value = string.Join(", ", gv.TenLops);
            worksheet.Cells[i + 2, 11].Value = gv.IsActive ? "Hoạt động" : "Chưa hoạt động";
        }

        worksheet.Cells.AutoFitColumns();

        // Lưu file vào thư mục đã chọn
        await File.WriteAllBytesAsync(filePath, package.GetAsByteArray(), cancellationToken);

        // Trả về đường dẫn file đã lưu
        return new FileContentResult(await File.ReadAllBytesAsync(filePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = fileName
        };
    }
}


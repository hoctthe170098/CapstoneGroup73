using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination.GetGiaoViensWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Commands.ImportGiaoViensFromExcel;
public record ImportGiaoViensFromExcelCommand : IRequest<Output>
{
    public required IFormFile File { get; init; }
}

public class ImportGiaoViensFromExcelCommandHandler : IRequestHandler<ImportGiaoViensFromExcelCommand, Output>
{
    private readonly IApplicationDbContext _context;

    public ImportGiaoViensFromExcelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Output> Handle(ImportGiaoViensFromExcelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.File == null || request.File.Length == 0)
                throw new NotFoundDataException("File không được để trống");

            var giaoVienList = new List<GiaoVienDto>();

            using (var stream = new MemoryStream())
            {
                await request.File.CopyToAsync(stream, cancellationToken);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new FormatException("File Excel không hợp lệ hoặc không có sheet nào.");

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)  // Bỏ qua hàng tiêu đề
                {
                    if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 2].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 4].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 5].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 6].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 7].Text) ||  
                    string.IsNullOrWhiteSpace(worksheet.Cells[row, 8].Text))  
                    {
                        throw new WrongInputException($"Import thất bại: Dòng {row} có ô bị trống. Hãy kiểm tra lại file Excel.");
                    }

                    // Kiểm tra định dạng ngày sinh hợp lệ
                    if (!DateOnly.TryParseExact(worksheet.Cells[row, 4].Text.Trim(), "yyyy-MM-dd", out DateOnly ngaySinh))
                    {
                        throw new WrongInputException($"Import thất bại: Dòng {row} - Ngày sinh không hợp lệ. Định dạng phải là yyyy-MM-dd.");
                    }

                    var giaoVien = new GiaoVienDto
                    {
                        Code = worksheet.Cells[row, 1].Text.Trim(),
                        Ten = worksheet.Cells[row, 2].Text.Trim(),
                        GioiTinh = worksheet.Cells[row, 3].Text.Trim(),
                        NgaySinh = DateOnly.ParseExact(worksheet.Cells[row, 4].Text.Trim(), "yyyy-MM-dd"),
                        Email = worksheet.Cells[row, 5].Text.Trim(),
                        SoDienThoai = worksheet.Cells[row, 6].Text.Trim(),
                        DiaChi = worksheet.Cells[row, 7].Text.Trim(),
                        TruongDangDay = worksheet.Cells[row, 8].Text.Trim(),
                    };

                    giaoVienList.Add(giaoVien);
                }
            }

            return new Output
            {
                isError = false,
                data = giaoVienList,
                code = 200
            };
        }
        catch
        {
            throw new WrongInputException();
        }
    }
}

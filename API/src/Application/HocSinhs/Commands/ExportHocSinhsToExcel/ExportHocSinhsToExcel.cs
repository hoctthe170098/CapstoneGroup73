using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.HocSinhs.Queries.GetHocViensWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.HocSinhs.Commands.ExportHocSinhsToExcel;
public record ExportHocSinhsToExcelCommand : IRequest<Output>
{
    public required List<HocSinhDto> HocSinhs { get; init;}
}

public class ExportHocSinhsToExcelCommandHandler : IRequestHandler<ExportHocSinhsToExcelCommand, Output>
{
    public Task<Output> Handle(ExportHocSinhsToExcelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.HocSinhs.Any())
            {
                throw new NotFoundDataException("Không có học viên nào để xuất.");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachHocSinh");

                var headers = new[] { "Mã HS", "Tên", "Giới Tính", "Địa Chỉ", "Lớp", "Trường Đang Học",  "Ngày Sinh", "Email", "Số Điện Thoại", "Cơ Sở", "Chính Sách", "Danh Sách Lớp" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                for (int i = 0; i < request.HocSinhs.Count; i++)
                {                   
                    var hs = request.HocSinhs[i];
                    worksheet.Cells[i + 2, 1].Value = hs.Code;
                    worksheet.Cells[i + 2, 2].Value = hs.Ten;
                    worksheet.Cells[i + 2, 3].Value = hs.GioiTinh;
                    worksheet.Cells[i + 2, 4].Value = hs.DiaChi;
                    worksheet.Cells[i + 2, 5].Value = hs.Lop;
                    worksheet.Cells[i + 2, 6].Value = hs.TruongDangHoc;
                    worksheet.Cells[i + 2, 7].Value = hs.NgaySinh.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 8].Value = hs.Email;
                    worksheet.Cells[i + 2, 9].Value = hs.SoDienThoai;
                    worksheet.Cells[i + 2, 10].Value = hs.TenCoSo;
                    worksheet.Cells[i + 2, 11].Value = hs.TenChinhSach;
                    worksheet.Cells[i + 2, 12].Value = string.Join(", ", hs.TenLops);
                }

                worksheet.Cells.AutoFitColumns();

                string fileName = "DanhSachHocSinh.xlsx";

                return Task.FromResult(new Output
                {
                    isError = false,
                    data = new FileContentResult(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = fileName
                    },
                    message = "File downloaded successfully."
                });
            }
        }
        catch
        {
            return Task.FromResult(new Output
            {
                isError = true,
                message = "Đã xảy ra lỗi trong khi xuất tệp."
            });
        }
    }
}

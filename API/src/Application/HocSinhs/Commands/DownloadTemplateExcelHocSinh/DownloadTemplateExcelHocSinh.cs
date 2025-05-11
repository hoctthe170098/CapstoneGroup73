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

namespace StudyFlow.Application.HocSinhs.Commands.DownloadTemplateExcelHocSinh;
public record DownloadTemplateExcelHocSinhCommand : IRequest<Output>
{
}

public class DownloadTemplateExcelHocSinhCommandHandler : IRequestHandler<DownloadTemplateExcelHocSinhCommand, Output>
{
    public Task<Output> Handle(DownloadTemplateExcelHocSinhCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachHocSinh");

                var headers = new[] { "Mã HS", "Tên", "Giới Tính", "Địa Chỉ", "Lớp", "Trường Đang Học",  "Ngày Sinh", "Email", "Số Điện Thoại", "Chính Sách" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }
                worksheet.Cells.AutoFitColumns();

                string fileName = "TemplateDanhSachHocSinh.xlsx";

                return Task.FromResult(new Output
                {
                    isError = false,
                    data = new FileContentResult(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = fileName
                    },
                    message = "Tải template thành công."
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

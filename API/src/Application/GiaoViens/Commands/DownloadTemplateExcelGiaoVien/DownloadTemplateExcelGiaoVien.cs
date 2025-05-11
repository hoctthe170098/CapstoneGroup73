namespace StudyFlow.Application.GiaoViens.Commands.DownloadTemplateExcelGiaoVien;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.DownloadTemplateExcelGiaoVien;
using StudyFlow.Domain.Entities;

public record DownloadTemplateExcelGiaoVienCommand : IRequest<Output>
{
}

public class DownloadTemplateExcelGiaoVienCommandHandler : IRequestHandler<DownloadTemplateExcelGiaoVienCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DownloadTemplateExcelGiaoVienCommandHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContext)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContext;
    }

    public Task<Output> Handle(DownloadTemplateExcelGiaoVienCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachGiaoVien");

                var headers = new[] { "Mã GV", "Tên", "Giới Tính", "Ngày Sinh", "Email", "Số Điện Thoại", "Địa Chỉ", "Trường Đang Dạy"};
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }
                worksheet.Cells.AutoFitColumns();

                string fileName = "TemplateDanhSachGiaoVien.xlsx";

                return Task.FromResult(new Output
                {
                    isError = false,
                    data = new FileContentResult(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = fileName
                    },
                    message = "Tải file thành công."
                });
            }
        }
        catch
        {
            return Task.FromResult(new Output
            {
                isError = true,
                message = "Xảy ra lỗi khi xuất file."
            });
        }
    }
}



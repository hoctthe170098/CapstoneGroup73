namespace StudyFlow.Application.GiaoViens.Commands.ExportGiaoViensToExcel;
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
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination.GetGiaoViensWithPagination;

public record ExportGiaoViensToExcelCommand : IRequest<Output>
{
}

public class ExportGiaoVienCommandHandler : IRequestHandler<ExportGiaoViensToExcelCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExportGiaoVienCommandHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContext)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContext;
    }

    public async Task<Output> Handle(ExportGiaoViensToExcelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
            var coSoId = _identityService.GetCampusId(token);

            var giaoViens = await _context.GiaoViens
            //.Include(gv => gv.Coso)
            .Include(gv => gv.LichHocs)
            .Where(gv => gv.CoSoId == coSoId)   
            .ToListAsync(cancellationToken);

            if (!giaoViens.Any())
            {
                throw new NotFoundDataException("Không có giáo viên nào để xuất.");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachGiaoVien");

                var headers = new[] { "Mã GV", "Tên", "Giới Tính", "Ngày Sinh", "Email", "Số Điện Thoại", "Địa Chỉ", "Trường Đang Dạy", "Danh Sách Lớp", "Trạng Thái" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                for (int i = 0; i < giaoViens.Count; i++)
                {
                    var gv = giaoViens[i];
                    bool isActive = gv.UserId != null && await _identityService.IsUserActiveAsync(gv.UserId);
                    var lopDangDay = gv.LichHocs
                        .Where(lh => lh.GiaoVienCode == giaoViens[i].Code && lh.TrangThai == "Cố định")
                        .Select(lh => lh.TenLop)
                        .Distinct()
                        .ToList();

                    worksheet.Cells[i + 2, 1].Value = gv.Code;
                    worksheet.Cells[i + 2, 2].Value = gv.Ten;
                    worksheet.Cells[i + 2, 3].Value = gv.GioiTinh;
                    worksheet.Cells[i + 2, 4].Value = gv.NgaySinh.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = gv.Email;
                    worksheet.Cells[i + 2, 6].Value = gv.SoDienThoai;
                    worksheet.Cells[i + 2, 7].Value = gv.DiaChi;
                    worksheet.Cells[i + 2, 8].Value = gv.TruongDangDay;
                    worksheet.Cells[i + 2, 9].Value = lopDangDay.Any() ? string.Join(", ", lopDangDay) : "Không có lớp"; 
                    worksheet.Cells[i + 2, 10].Value = isActive ? "Hoạt động" : "Chưa hoạt động";
                }

                worksheet.Cells.AutoFitColumns();

                string fileName = "DanhSachGiaoVien.xlsx";

                return new Output
                {
                    isError = false,
                    data = new FileContentResult(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = fileName
                    },
                    message = "File downloaded successfully."
                };
            }
        }
        catch
        {
            return new Output
            {
                isError = true,
                message = "Xảy ra lỗi khi xuất file."
            };
        }
    }
}



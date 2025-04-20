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
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.KetQuaBaiKiemTras.Commands.ExportKetQuaBaiKiemTraToExcel;
public record ExportKetQuaBaiKiemTraToExcelCommand : IRequest<Output>
{
    public required string BaiKiemTraId { get; init; }
}

public class ExportKetQuaBaiKiemTraToExcelCommandHandler : IRequestHandler<ExportKetQuaBaiKiemTraToExcelCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ExportKetQuaBaiKiemTraToExcelCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(ExportKetQuaBaiKiemTraToExcelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ketQuaKiemTra = await _context.KetQuaBaiKiemTras
                .Where(kq => kq.BaiKiemTraId.ToString() == request.BaiKiemTraId)
                .ProjectTo<KetQuaKiemTraExcelDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!ketQuaKiemTra.Any())
            {
                throw new NotFoundDataException("Không có dữ liệu nào để xuất.");
            }

            using (var package = new ExcelPackage())
            {
                var baiKiemTra = await _context.BaiKiemTras.FirstOrDefaultAsync(kt => kt.Id.ToString() == request.BaiKiemTraId, cancellationToken);
                var worksheet = package.Workbook.Worksheets.Add(baiKiemTra!.Ten + " " + baiKiemTra.NgayKiemTra);

                var headers = new[] { "Id", "Học Sinh Code", "Họ và Tên", "Điểm", "Nhận Xét" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                for (int i = 0; i < ketQuaKiemTra.Count; i++)
                {
                    var kq = ketQuaKiemTra[i];

                    worksheet.Cells[i + 2, 1].Value = kq.Id;
                    worksheet.Cells[i + 2, 2].Value = kq.HocSinhCode;
                    worksheet.Cells[i + 2, 3].Value = kq.TenHocSinh;
                    worksheet.Cells[i + 2, 4].Value = kq.Diem == null ? "Chưa có điểm" : kq.Diem;
                    worksheet.Cells[i + 2, 5].Value = kq.NhanXet == null ? "Chưa có nhận xét" : kq.NhanXet;
                }

                worksheet.Cells.AutoFitColumns();

                string fileName = baiKiemTra.Ten + " (" + baiKiemTra.NgayKiemTra + ")" + ".xlsx";

                return new Output
                {
                    isError = false,
                    code = 200,
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

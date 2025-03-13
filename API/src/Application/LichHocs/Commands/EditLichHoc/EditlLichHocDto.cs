using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;
public class EditlLichHocDto
{
    public Guid Id { get; set; }
    public int? Thu { get; set; }
    public int? PhongId { get; set; }
    public string? TenLop { get; set; }
    public string? GioBatDau { get; set; }
    public string? GioKetThuc { get; set; }
    public string? NgayBatDau { get; set; }
    public string? NgayKetThuc { get; set; }
    public int? HocPhi { get; set; }
    public string? TrangThai { get; set; }
    public string? GiaoVienCode { get; set; }
    public int? ChuongTrinhId { get; set; }
}

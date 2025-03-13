using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
public class CreateLichHocDto
{
    public int Thu { get; set; }
    public int PhongId { get; set; }
    public string TenLop { get; set; } = string.Empty;
    public string GioBatDau { get; set; } = string.Empty;
    public string GioKetThuc { get; set; } = string.Empty;
    public string NgayBatDau { get; set; } = string.Empty;
    public string NgayKetThuc { get; set; } = string.Empty;
    public int HocPhi { get; set; }
    public string TrangThai { get; set; } = "NotYet";
    public string GiaoVienCode { get; set; } = string.Empty;
    public int ChuongTrinhId { get; set; }
}

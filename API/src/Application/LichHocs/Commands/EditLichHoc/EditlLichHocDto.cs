using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Commands.EditLichHoc;
public class EditLichHocDto
{
    public required string TenLop { get; init; }
    public required string GiaoVienCode { get; init; }
    public required DateOnly NgayBatDau { get; init; }
    public required DateOnly NgayKetThuc { get; init; }
    public int HocPhi { get; set; }
    public int ChuongTrinhId { get; set; }
    public required List<LichHocDto> LichHocs { get; init; }
    public required List<string> HocSinhCodes { get; init; }
}
public class LichHocDto
{
    public Guid? Id { get; set; }
    public int Thu { get; init; }
    public int PhongId { get; init; }
    public required string GioBatDau { get; init; }
    public required string GioKetThuc { get; init; }
}

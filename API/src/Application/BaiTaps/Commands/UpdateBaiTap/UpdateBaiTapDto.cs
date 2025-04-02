using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;
public class UpdateBaiTapDto
{
    public Guid Id { get; set; }
    public DateOnly NgayTao { get; set; }
    public required Guid LichHocId { get; set; }
    public required string TieuDe { get; set; }
    public required string NoiDung { get; set; }
    public DateTime? ThoiGianKetThuc { get; set; }
    public string? UrlFile { get; set; }
    public required string TrangThai { get; set; }
}
public class LichHocDropdownDto
{
    public Guid Id { get; set; }
    public string TenLop { get; set; } = string.Empty;
}

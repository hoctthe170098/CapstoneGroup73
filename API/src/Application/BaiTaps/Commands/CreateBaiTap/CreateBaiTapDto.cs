using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
public class CreateBaiTapDto
{
    public Guid LichHocId { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateTime? ThoiGianKetThuc { get; set; }  
    public string? UrlFile { get; set; }
}
public class LichHocDropdownDto
{
    public Guid Id { get; set; }
    public string TenLop { get; set; } = string.Empty;
}

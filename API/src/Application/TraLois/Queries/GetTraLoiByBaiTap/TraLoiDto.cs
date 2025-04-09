using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.TraLois.Queries.GetBaiTapByTraLoi;
public class TraLoiDto
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; } = default!;
    public string? UrlFile { get; set; }
    public DateTime ThoiGian { get; set; }
    public string HocSinhTen { get; set; } = default!;
}

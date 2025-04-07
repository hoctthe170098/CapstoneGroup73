using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
public class CreateBaiTapDto
{
    public string TenLop { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateTime? ThoiGianKetThuc { get; set; }  
    public required IFormFile TaiLieu { get; set; }

}


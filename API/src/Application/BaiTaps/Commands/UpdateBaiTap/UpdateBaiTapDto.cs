using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;
public class UpdateBaiTapDto
{
    public Guid Id { get; set; }
    public required string TieuDe { get; set; }
    public required string NoiDung { get; set; }
    public DateTime? ThoiGianKetThuc { get; set; }
    public IFormFile? TaiLieu { get; set; }
    public required string TrangThai { get; set; }
}

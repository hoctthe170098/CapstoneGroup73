using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
public class CreateChuongTrinhDto
{
    public required string TieuDe { get; set; }
    public required string MoTa { get; set; }
    public List<CreateNoiDungBaiHocDto>? NoiDungBaiHocs { get; set; } = new List<CreateNoiDungBaiHocDto>();
}

public class CreateNoiDungBaiHocDto
{
    public required string TieuDe { get; set; }
    public required string Mota { get; set; }
    public required int SoThuTu { get; set; }
    public List<CreateTaiLieuHocTapDto>? TaiLieuHocTaps { get; set; } = new List<CreateTaiLieuHocTapDto>();
}

public class CreateTaiLieuHocTapDto
{
    public required string urlType { get; set; }
    public IFormFile? File { get; set; } // Thêm thuộc tính này
}

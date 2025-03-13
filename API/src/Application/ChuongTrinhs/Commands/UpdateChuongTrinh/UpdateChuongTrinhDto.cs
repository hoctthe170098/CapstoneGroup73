using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
public class UpdateChuongTrinhDto
{
    public required int Id { get; set; }
    public required string TieuDe { get; set; }
    public required string MoTa { get; set; }
    public List<UpdateNoiDungBaiHocDto>? NoiDungBaiHocs { get; set; } = new List<UpdateNoiDungBaiHocDto>();
}
public class UpdateNoiDungBaiHocDto
{
    public string? Id { get; set; }
    public required string TieuDe { get; set; }
    public required string Mota { get; set; }
    public required int SoThuTu { get; set; }
    public List<UpdateTaiLieuHocTapDto>? TaiLieuHocTaps { get; set; } = new List<UpdateTaiLieuHocTapDto>();
}

public class UpdateTaiLieuHocTapDto
{
    public string? Id { get; set; }
    public required string urlType { get; set; }
    public IFormFile? File { get; set; } // Thêm thuộc tính này
}

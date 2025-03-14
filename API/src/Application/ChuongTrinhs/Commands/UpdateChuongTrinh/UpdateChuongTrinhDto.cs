using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
public class UpdateChuongTrinhDto
{
    public required int Id { get; init; }
    public required string TieuDe { get; init; }
    public required string MoTa { get; init; }
    public required string TrangThai {  get; init; }
    public List<UpdateNoiDungBaiHocDto>? NoiDungBaiHocs { get; init; } = new List<UpdateNoiDungBaiHocDto>();
}
public class UpdateNoiDungBaiHocDto
{
    public string? Id { get; init; }
    public required string TieuDe { get; init; }
    public required string Mota { get; init; }
    public required int SoThuTu { get; init; }
    public List<UpdateTaiLieuHocTapDto>? TaiLieuHocTaps { get; init; } = new List<UpdateTaiLieuHocTapDto>();
}

public class UpdateTaiLieuHocTapDto
{
    public string? Id { get; init; }
    public required string urlType { get; init; }
    public IFormFile? File { get; init; } // Thêm thuộc tính này
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
public class CreateChuongTrinhDto
{
    public required string TieuDe { get; init; }
    public required string MoTa { get; init; }
    public List<CreateNoiDungBaiHocDto>? NoiDungBaiHocs { get; init; } = new List<CreateNoiDungBaiHocDto>();
}

public class CreateNoiDungBaiHocDto
{
    public required string TieuDe { get; init; }
    public required string Mota { get; init; }
    public required int SoThuTu { get; init; }
    public List<CreateTaiLieuHocTapDto>? TaiLieuHocTaps { get; init; } = new List<CreateTaiLieuHocTapDto>();
}

public class CreateTaiLieuHocTapDto
{
    public required string urlType { get; init; }
    public IFormFile? File { get; init; } // Thêm thuộc tính này
}

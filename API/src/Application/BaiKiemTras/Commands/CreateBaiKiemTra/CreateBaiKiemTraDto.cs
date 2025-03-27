using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.CreateBaiKiemTra;

public class UpdateBaiKiemTraDto
{
    public required string TenBaiKiemTra {  get; init; }
    public required DateOnly NgayKiemTra { get; init; }
    public required string TenLop {  get; init; }
    public required IFormFile TaiLieu { get; init; }
}

using Microsoft.AspNetCore.Http;

namespace StudyFlow.Application.ChuongTrinhs.Commands.UpdateBaiKiemTra;

public class UpdateBaiKiemTraDto
{
    public required Guid Id { get; init; }
    public required string TenBaiKiemTra {  get; init; }
    public required DateOnly NgayKiemTra { get; init; }
    public required string TenLop {  get; init; }
    public IFormFile? TaiLieu { get; init; }
}

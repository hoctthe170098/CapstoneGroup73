using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;

public class BaiTapGiaoVienDto
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateOnly NgayTao { get; set; }
    public DateTime? ThoiGianKetThuc { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? TenLop { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BaiTap, BaiTapGiaoVienDto>()
                .ForMember(dest => dest.TenLop, opt => opt.MapFrom(src => src.LichHoc.TenLop));
        }
    }
}

using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;
public class BaiTapWithPaginationDTO
{
    public int PageNumber { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<BaiTapGroupByTenLopDto> Items { get; set; } = new();
}
public class BaiTapGroupByTenLopDto
{
    public string TenLop { get; set; } = string.Empty;
    public List<BaiTapGiaoVienDto> BaiTaps { get; set; } = new();
}
public class BaiTapGiaoVienDto
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
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

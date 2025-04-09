using AutoMapper;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiTaps.Queries.TeacherAssignmentList;

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
    public List<BaiTapDto> BaiTaps { get; set; } = new();
}

public class BaiTapDto
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateOnly NgayTao { get; set; }
    public DateTime? ThoiGianKetThuc { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? UrlFile { get; set; }
    public string? GiaoVien { get; set; }
    public string? TenLop { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BaiTap, BaiTapDto>()
                .ForMember(dest => dest.GiaoVien, opt => opt.MapFrom(src => src.LichHoc.GiaoVien.Ten))
                .ForMember(dest => dest.TenLop, opt => opt.MapFrom(src => src.LichHoc.TenLop));
        }
    }
}

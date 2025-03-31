using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiTaps.Queries.TeacherAssignmentList;
public class BaiTapDto
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateOnly Ngay { get; set; }
    public TimeOnly ThoiGianBatDau { get; set; }
    public TimeOnly ThoiGianKetThuc { get; set; }
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

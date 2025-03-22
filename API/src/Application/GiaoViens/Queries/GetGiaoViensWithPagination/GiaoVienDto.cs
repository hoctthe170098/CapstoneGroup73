using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination.GetGiaoViensWithPagination;
public class GiaoVienDto
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string TruongDangDay { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string? TenCoSo { get; set; }
    public IList<string> TenLops { get; set; } = new List<string>();
    public bool IsActive { get; set; }  
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GiaoVien, GiaoVienDto>()
                .ForMember(dest => dest.TenCoSo, opt => opt.MapFrom(src => src.Coso.Ten))
                .ForMember(dest => dest.TenLops, opt => opt.MapFrom(src => src.LichHocs.Select(lh => lh.TenLop).Distinct().ToList()))
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        }
    }
}


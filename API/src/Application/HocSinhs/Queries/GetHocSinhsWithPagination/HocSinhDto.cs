using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.HocSinhs.Queries.GetHocViensWithPagination;
public class HocSinhDto
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string Lop { get; set; }
    public required string TruongDangHoc { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public required string TenCoSo { get; set; }
    public string? TenChinhSach { get; set; }
    public string? UserId { get; set; }
    public bool IsActive { get; set; }
    public IList<string> TenLops { get; set; } = new List<string>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<HocSinh, HocSinhDto>()
                .ForMember(dest => dest.TenCoSo, opt => opt.MapFrom(src => src.Coso.Ten))
                .ForMember(dest => dest.TenChinhSach, opt => opt.MapFrom(src => src.ChinhSach.Ten))
                .ForMember(dest => dest.TenLops, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        }
    }
}

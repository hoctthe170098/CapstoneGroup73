using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;
public class NhanVienDto
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public required string TenCoSo { get; set; }
    public string? TenVaiTro { get; set; }
    public bool TrangThai { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<NhanVien, NhanVienDto>()
                .ForMember(dest => dest.TenCoSo, opt => opt.MapFrom(src => src.Coso.Ten))
                .ForMember(dest => dest.TenVaiTro, opt => opt.Ignore());
        }
    }
}

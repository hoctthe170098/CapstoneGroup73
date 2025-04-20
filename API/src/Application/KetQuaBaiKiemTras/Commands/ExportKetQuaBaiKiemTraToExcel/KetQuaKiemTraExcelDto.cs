using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.KetQuaBaiKiemTras.Commands.ExportKetQuaBaiKiemTraToExcel;
public class KetQuaKiemTraExcelDto
{
    public Guid Id { get; set; }
    public required string HocSinhCode { get; set; }
    public string? TenHocSinh { get; set; }
    public float? Diem { get; set; }
    public string? NhanXet { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<KetQuaBaiKiemTra, KetQuaKiemTraExcelDto>()
                .ForMember(dest => dest.TenHocSinh, opt => opt.MapFrom(src => src.HocSinh.Ten));
        }
    }
}

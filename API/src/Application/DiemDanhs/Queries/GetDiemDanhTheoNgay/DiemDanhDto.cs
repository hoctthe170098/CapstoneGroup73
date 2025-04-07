using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.DiemDanhs.Queries.GetDiemDanhTheoNgay
{
    class DiemDanhDto
    {
        public Guid Id { get; set; }//k
        public string HocSinhCode { get; set; } = "";
        public string TenHocSinh { get; set; } = "";
        public required string TrangThai { get; set; }
        public float? DiemBTVN { get; set; }
        public float? DiemTrenLop { get; set; }
        public string? NhanXet { get; set; }
        public required Guid ThamGiaLopHocId { get; set; }
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<DiemDanh, DiemDanhDto>()
                    .ForMember(dest => dest.HocSinhCode, opt => opt.MapFrom(src => src.ThamGiaLopHoc.HocSinh.Code))
                    .ForMember(dest => dest.TenHocSinh, opt => opt.MapFrom(src => src.ThamGiaLopHoc.HocSinh.Ten));
            }
        }
    }
}

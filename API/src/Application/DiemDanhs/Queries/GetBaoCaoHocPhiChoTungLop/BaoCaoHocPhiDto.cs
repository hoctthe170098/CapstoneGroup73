using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoHocPhiChoTungLop
{
    public class BaoCaoHocPhiDto
    {
        public int Thang { get; set; }
        public int Nam {  get; set; }
        public List<HocPhiDto> HocPhis { get; set; } = new List<HocPhiDto>();
        public required List<ThangNamDto> ThangNams { get; set; } 
    }
    public class HocPhiDto
    {
        public required string HocSinhCode { get; set; }
        public required string TenHocSinh { get; set; }
        public int SoBuoiHoc {  get; set; }
        public int SoBuoiNghi { get; set; }
        public int HocPhi1Buoi { get; set; }
        public int SoPhanTramGiam { get; set; }
        public float TongHocPhi  { get; set; }
    }
}

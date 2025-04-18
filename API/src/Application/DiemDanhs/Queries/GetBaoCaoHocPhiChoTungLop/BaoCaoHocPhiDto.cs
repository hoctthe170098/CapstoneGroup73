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
        public List<HocPhiDto> DiemDanhs { get; set; } = new List<HocPhiDto>();
        public int ThangBatDau { get; set; }
        public int ThangKetThuc {  get; set; }
    }
    public class HocPhiDto
    {
        public required string HocSinhCode { get; set; }
        public required string TenHocSinh { get; set; }
        public int SoBuoiHoc {  get; set; }
        public int SoBuoiNghi { get; set; }
        public int HocPhi1Buoi { get; set; }
        public int TongHocPhi  { get; set; }
    }
}

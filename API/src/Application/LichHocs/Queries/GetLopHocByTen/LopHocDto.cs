using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.HocSinhs.Queries.GetHocSinhsByNameOrCode;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocByTen
{
    class LopHocDto
    {
        public required string TenLop { get; init; }
        public required DateOnly NgayBatDau { get; init; }
        public required DateOnly NgayKetThuc { get; init; }
        public int HocPhi { get; set; }
        public required string GiaoVienCode { get; init; }
        public int ChuongTrinhId { get; set; }
        public required List<LichHocDto> LichHocs { get; init; } = new List<LichHocDto>();
        public required List<HocSinhDto> HocSinhs { get; set; } = new List<HocSinhDto>();
        public required bool DaHoc {  get; set; }
    }
    public class LichHocDto
    {
        public Guid Id { get; set; }
        public int Thu { get; init; }
        public int PhongId { get; init; }
        public required TimeOnly GioBatDau { get; init; }
        public required TimeOnly GioKetThuc { get; init; }
    }
}

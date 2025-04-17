using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemDanhHocSinh
{
    public class DiemDanhDto
    {
        public DateOnly Ngay { get; set; }
        public TimeOnly ThoiGianBatDau { get; set; }
        public TimeOnly ThoiGianKetThuc {  get; set; }
        public required string TenPhong {  get; set; }
        public required string TenGiaoVien {  get; set; }
        public required string TinhTrangDiemDanh {  get; set; }
    }
}

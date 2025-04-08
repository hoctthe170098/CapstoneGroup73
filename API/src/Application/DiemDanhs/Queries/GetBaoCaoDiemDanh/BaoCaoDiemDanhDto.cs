using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemDanh
{
    public class BaoCaoDiemDanhDto
    {
        public DateOnly Ngay { get; set; }
        public List<DiemDanhDto> DiemDanhs { get; set; } = new List<DiemDanhDto>();
    }
    public class DiemDanhDto
    {
        public string? HocSinhCode { get; set; }
        public string? TenHocSinh { get; set; }
        public string? TrangThai {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemHangNgayCuaHocSinh
{
    public class DiemDanhDto
    {
        public DateOnly Ngay { get; set; }
        public List<DiemDTO> DiemDanhs { get; set; } = new List<DiemDTO>();
    }
    public class DiemDTO
    {
        public string? HocSinhCode { get; set; }
        public string? TenHocSinh { get; set; }
        public float? DiemBTVN { get; set; }
        public float? DiemTrenLop { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Queries.GetLichHocHocSinh
{
    class GetLichHocHocSinhByThuVaNamDto
    {
        public int Nam { get; set; }
        public int Tuan { get; set; }
        public List<LichHocTrongNgayDto> LichHocCaTuans { get; set; } = new List<LichHocTrongNgayDto>();
    }
    class LichHocTrongNgayDto
    {      
        public int thu {  get; set; }
        public DateOnly Ngay { get; set; }
        public List<LopHocDto> Lops { get; set; } = new List<LopHocDto>();
    }
    class LopHocDto
    {
        public string TenLop { get; set; } = "";
        public string TenChuongTrinh { get; set; } = "";
        public string TenPhong { get; set; } = "";  
        public TimeOnly GioBatDau { get; set; }
        public TimeOnly GioKetThuc {  get; set; }
        public string TrangThai { get; set; } = "";
    }
}

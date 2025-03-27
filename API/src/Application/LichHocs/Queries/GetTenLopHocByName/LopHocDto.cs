using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Queries.GetTenLopHocByName
{
    class LopHocByNameDto
    {
        public required string TenLop { get; set; }
        public List<LichHocByNameDto> LichHocs { get; set; } = new List<LichHocByNameDto>();
    }
    class LichHocByNameDto
    {
        public int Thu { get; set; }
        public TimeOnly GioBatDau { get; set; }
        public TimeOnly GioKetThuc { get; set; }
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public required string TenPhong { get; set; }
        public required string TrangThai { get; set; }
    }
}

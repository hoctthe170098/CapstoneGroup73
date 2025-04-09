using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay
{
    public class UpdateDiemDanhDto
    {
        public Guid Id { get; set; }//k
        public required string HocSinhCode { get; set; } = "";
        public required string TenHocSinh { get; set; } = "";
        public required string TrangThai { get; set; }
        public float? DiemBTVN { get; set; }
        public float? DiemTrenLop { get; set; }
        public string? NhanXet { get; set; }
    }
}

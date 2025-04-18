using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemDanhChoTungLop
{
    public class BaoCaoHocPhiDto
    {
        public DateOnly Ngay { get; set; }
        public List<DiemDanhDto> DiemDanhs { get; set; } = new List<DiemDanhDto>();
        public List<DateOnly> Ngays { get; set;} = new List<DateOnly>();
    }
    public class DiemDanhDto
    {
        public Guid Id { get; set; }
        public string? HocSinhCode { get; set; }
        public string? TenHocSinh { get; set; }
        public string? TrangThai {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiKiemTras.Queries.GetLichKiemTraVaKetQuaChoHocSinh;
public class BaiKiemTraDto
{
    public required string Ten { get; set; }
    public required DateOnly NgayKiemTra { get; set; }
    public required string TrangThai { get; set; }
    public float? Diem {  get; set; }
    public string? NhanXet {  get; set; }
}


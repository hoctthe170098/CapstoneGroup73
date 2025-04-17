using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemHocSinh;
public class NhanXetDto
{
    public string Ngay { get; set; } = default!;
    public string? NhanXet { get; set; }
}
public class DiemChiTietDto
{
    public string Ngay { get; set; } = default!;
    public string Diem { get; set; } = default!;
    public string? NhanXet { get; set; }
}
public class DiemKiemTraDto
{
    public string Ten { get; set; } = default!;
    public string NgayKiemTra { get; set; } = default!;
    public string TrangThai { get; set; } = default!;
    public string Diem { get; set; } = default!;
    public string? NhanXet { get; set; }
}

public class BaoCaoHocSinhDto
{
    public string Ten { get; set; } = default!;
    public string Code { get; set; } = default!;
    public double DiemTrenLopTB { get; set; }
    public double DiemBaiTapTB { get; set; }
    public double DiemKiemTraTB { get; set; }
    public List<NhanXetDto> NhanXetDinhKy { get; set; } = new();
    public List<DiemChiTietDto> DiemTrenLopChiTiet { get; set; } = new();
    public List<DiemChiTietDto> DiemBaiTapVeNha { get; set; } = new();
    public List<DiemKiemTraDto> DiemKiemTra { get; set; } = new();
}

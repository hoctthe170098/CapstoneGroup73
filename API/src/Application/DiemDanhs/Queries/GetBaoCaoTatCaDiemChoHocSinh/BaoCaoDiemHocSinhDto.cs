namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoDiemHocSinh;

public class NhanXetDto
{
    public string Ngay { get; set; } = default!;
    public string? NhanXet { get; set; }
}

public class DiemHangNgayDto
{
    public string Ngay { get; set; } = default!;
    public string DiemTrenLop { get; set; } = "N/A";
    public string DiemBTVN { get; set; } = "N/A";
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
    public List<DiemHangNgayDto> DiemHangNgay { get; set; } = new();
}

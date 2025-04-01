using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class LichHoc
{
    public Guid Id { get; set; }
    public required int Thu { get; set; }
    public required int PhongId { get; set; }
    [JsonIgnore]
    public Phong Phong { get; set; } = null!;
    public required string TenLop { get; set; }
    public required TimeOnly GioBatDau {  get; set; }
    public required TimeOnly GioKetThuc { get; set; }
    public required DateOnly NgayBatDau { get; set; }
    public required DateOnly NgayKetThuc {  get; set; }
    public required int HocPhi { get; set; }
    public required string TrangThai { get; set; }
    public required string GiaoVienCode { get; set; }
    [JsonIgnore]
    public GiaoVien GiaoVien { get; set; } = null!;
    public required int ChuongTrinhId { get; set; }
    [JsonIgnore]
    public ChuongTrinh ChuongTrinh { get; set; } = null!;
    public Guid? LichHocGocId { get; set; }
    [JsonIgnore]
    public LichHoc LichHocGoc { get; set; } = null!;
    public DateOnly? NgayHocGoc { get; set; }
    [JsonIgnore]
    public IList<ThamGiaLopHoc> ThamGiaLopHocs { get; private set; } = new List<ThamGiaLopHoc>();
    [JsonIgnore]
    public IList<BaiTap> BaiTaps { get; private set; } = new List<BaiTap>();
    [JsonIgnore]
    public IList<BaiKiemTra> BaiKiemTras { get; private set; } = new List<BaiKiemTra>();

}

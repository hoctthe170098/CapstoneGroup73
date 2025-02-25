using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StudyFlow.Domain.Interfaces;


namespace StudyFlow.Domain.Entities;
public class HocSinh
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string Lop { get; set; }
    public required string TruongDangHoc { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public required Guid CoSoId { get; set; }
    [JsonIgnore]
    public CoSo Coso { get; set; } = null!;
    public int? ChinhSachId { get; set; }
    [JsonIgnore]
    public ChinhSach ChinhSach { get; set; } = null!;
    public string? UserId { get; set; }
    [JsonIgnore]
    public IList<ThamGiaLopHoc> ThamGiaLopHocs { get; private set; } = new List<ThamGiaLopHoc>();
    [JsonIgnore]
    public IList<TraLoi> TraLois { get; private set; } = new List<TraLoi>();
    [JsonIgnore]
    public IList<KetQuaBaiKiemTra> KetQuaBaiKiemTras { get; private set; } = new List<KetQuaBaiKiemTra>();
}

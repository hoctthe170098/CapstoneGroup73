using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class KetQuaBaiKiemTra
{
    public Guid Id { get; set; }
    public required string HocSinhCode { get; set; }//k
    [JsonIgnore]
    public HocSinh HocSinh { get; set; } = null!;//k
    public required Guid BaiKiemTraId { get; set; }
    [JsonIgnore]
    public BaiKiemTra BaiKiemTra { get; set; } = null!;
    public float? Diem { get; set; }
    public string? NhanXet  { get; set; }
}

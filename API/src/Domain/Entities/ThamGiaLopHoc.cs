using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class ThamGiaLopHoc
{
    public Guid Id { get; set; } //k
    public required DateOnly NgayBatDau { get; set; }
    public required DateOnly NgayKetThuc { get; set; }
    public required string TrangThai { get; set; }
    public required string HocSinhCode { get; set; }//u
    [JsonIgnore]
    public HocSinh HocSinh { get; set; } = null!;
    public required Guid LichHocId { get; set; }//u
    [JsonIgnore]
    public LichHoc LichHoc { get; set; } = null!;
    [JsonIgnore]
    public IList<DiemDanh> DiemDanhs { get; private set; } = new List<DiemDanh>();
}

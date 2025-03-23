using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class BaiKiemTra
{
    public Guid Id { get; set; }//k
    public required string Ten { get; set; }
    public required string UrlFile { get; set; }
    public required DateOnly NgayTao { get; set; }
    public DateOnly? NgayKiemTra { get; set; }
    public required string TrangThai {  get; set; }
    public Guid? LichHocId { get; set; }
    [JsonIgnore]
    public LichHoc LichHoc { get; set; } = null!;
    [JsonIgnore]
    public IList<KetQuaBaiKiemTra> KetQuaBaiKiemTras { get; private set; } = new List<KetQuaBaiKiemTra>();
}

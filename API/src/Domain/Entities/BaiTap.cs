using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class BaiTap
{
    public Guid Id { get; set; }//k
    public DateTime NgayTao { get; set; }//u
    public required Guid LichHocId { get; set; }//u
    public required string TieuDe { get; set; }
    public required string NoiDung { get; set; }
    public DateTime? ThoiGianKetThuc {  get; set; }
    public string? UrlFile { get; set; }
    public required string TrangThai { get; set; }
    [JsonIgnore]
    public LichHoc LichHoc { get; set; } = null!;
    [JsonIgnore]
    public IList<TraLoi> TraLois { get; private set; } = new List<TraLoi>();
}

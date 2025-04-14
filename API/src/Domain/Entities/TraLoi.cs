using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class TraLoi
{
    public Guid Id { get; set; }//k
    public required DateTime ThoiGian { get; set; }
    public required string NoiDung { get; set; }
    public string? UrlFile { get; set; }
    public required string HocSinhCode { get; set; }
    public int? Diem {  get; set; }
    public string? NhanXet {  get; set; }
    [JsonIgnore]
    public HocSinh HocSinh { get; set; } = null!;
    public required Guid BaiTapId { get; set; }
    [JsonIgnore]
    public BaiTap Baitap { get; set; } = null!;
}

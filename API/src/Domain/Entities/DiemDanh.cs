using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class DiemDanh
{
    public Guid Id { get; set; }//k
    public DateOnly Ngay { get; set; }//u
    public required Guid ThamGiaLopHocId { get; set; }//u
    [JsonIgnore]
    public ThamGiaLopHoc ThamGiaLopHoc { get; set; } = null!;
    public required string TrangThai { get; set; }
    public float? DiemBTVN { get; set; }
    public float? DiemTrenLop { get; set; }
    public string? NhanXet { get; set; }
}

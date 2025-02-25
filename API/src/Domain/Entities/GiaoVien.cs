using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StudyFlow.Domain.Interfaces;


namespace StudyFlow.Domain.Entities;
public class GiaoVien
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required string TruongDangDay { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public required Guid CoSoId { get; set; }
    [JsonIgnore]
    public CoSo Coso { get; set; } = null!;
    public string? UserId { get; set; }
    [JsonIgnore]
    public IList<LichHoc> LicHocs { get; private set; } = new List<LichHoc>();
}

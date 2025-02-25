using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class CoSo
{
    public Guid Id { get; set; }
    public required string Ten { get; set; }
    public required string DiaChi { get; set; }
    public required string SoDienThoai { get; set; }
    public required string TrangThai { get; set; }
    public required bool Default { get; set; }
    [JsonIgnore]
    public IList<NhanVien> NhanViens { get; private set; } = new List<NhanVien>();
    [JsonIgnore]
    public IList<GiaoVien> GiaoViens { get; private set; } = new List<GiaoVien>();
    [JsonIgnore]
    public IList<HocSinh> HocSinhs { get; private set; } = new List<HocSinh>();
}

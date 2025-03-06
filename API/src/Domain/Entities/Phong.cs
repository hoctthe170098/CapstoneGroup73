using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class Phong
{
    public int Id { get; set; }
    public required string Ten { get; set; }
    public required string TrangThai { get; set; }
    public required Guid CoSoId { get; set; }
    [JsonIgnore]
    public CoSo CoSo { get; set; } = null!;
    [JsonIgnore]
    public IList<LichHoc> LichHocs { get; private set; } = new List<LichHoc>();
}

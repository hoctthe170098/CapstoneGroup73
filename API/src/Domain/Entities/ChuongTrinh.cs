using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class ChuongTrinh
{
    public int Id { get; set; }
    public required string TieuDe { get; set; }
    public required string MoTa { get; set; }
    [JsonIgnore]
    public IList<LichHoc> LichHocs { get; private set; } = new List<LichHoc>();
    [JsonIgnore]
    public IList<NoiDungBaiHoc> NoiDungBaiHocs { get; private set; } = new List<NoiDungBaiHoc>();
}

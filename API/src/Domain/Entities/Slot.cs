using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class Slot
{
    public int Id { get; set; }
    public required string Ten { get; set; }
    public required TimeOnly BatDau { get; set; }
    public required TimeOnly KetThuc { get; set; }
    [JsonIgnore]
    public IList<LichHoc> LicHocs { get; private set; } = new List<LichHoc>();
}

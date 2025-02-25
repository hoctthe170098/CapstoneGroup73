using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class ChinhSach
{
    public int Id { get; set; }//k
    public required string Ten { get; set; }
    public required string Mota { get; set; }
    public required float PhanTramGiam { get; set; }
    [JsonIgnore]
    public IList<HocSinh> HocSinhs { get; private set; } = new List<HocSinh>();
}

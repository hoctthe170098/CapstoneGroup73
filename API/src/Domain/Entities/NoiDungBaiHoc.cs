using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class NoiDungBaiHoc
{
    public Guid Id { get; set; }//k
    public required string TieuDe { get; set; }
    public required string Mota { get; set; }
    public required int ChuongTrinhId { get; set; }
    [JsonIgnore]
    public ChuongTrinh ChuongTrinh { get; set; } = null!;
    [JsonIgnore]
    public IList<TaiLieuHocTap> TaiLieuHocTaps { get; private set; } = new List<TaiLieuHocTap>();
}

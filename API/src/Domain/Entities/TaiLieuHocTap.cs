using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class TaiLieuHocTap
{
    public Guid Id { get; set; }
    public required string Ten { get; set; }
    public required DateOnly NgayTao { get;set; }
    public required string urlType { get; set; }
    public required string urlFile { get; set; }
    public required Guid NoiDungBaiHocId { get; set; }
    [JsonIgnore]
    public NoiDungBaiHoc NoiDungBaiHoc { get; set; } = null!;
}

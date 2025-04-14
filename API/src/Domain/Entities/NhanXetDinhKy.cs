using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyFlow.Domain.Entities;
public class NhanXetDinhKy
{
    public Guid Id { get; set; }
    public int STT {  get; set; }
    public DateOnly NgayNhanXet {  get; set; }
    public required string NoiDungNhanXet { get; set; }
    public required Guid ThamGiaLopHocId {  get; set; }
    [JsonIgnore]
    public ThamGiaLopHoc ThamGiaLopHoc { get; set; } = null!;
}

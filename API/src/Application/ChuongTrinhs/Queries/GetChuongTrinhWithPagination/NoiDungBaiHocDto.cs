using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhWithPagination;
public class NoiDungBaiHocDto
{
    public Guid Id { get; set; }//k
    public required string TieuDe { get; set; }
    public required int SoThuTu { get; set; }
    public required string Mota { get; set; }
    public IList<TaiLieuHocTapDto> TaiLieuHocTaps { get; private set; } = new List<TaiLieuHocTapDto>();
}

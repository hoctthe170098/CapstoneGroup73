using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhWithPagination;
public class TaiLieuHocTapDto
{
    public Guid Id { get; set; }
    public required string Ten { get; set; }
    public required string urlType { get; set; }
    public required string urlFile { get; set; }
}

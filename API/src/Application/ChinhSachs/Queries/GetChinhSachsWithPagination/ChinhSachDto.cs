using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.ChinhSachs.Queries.GetChinhSachsWithPagination;
public class ChinhSachDto
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string Mota { get; set; } = string.Empty;
    public float PhanTramGiam { get; set; }
}

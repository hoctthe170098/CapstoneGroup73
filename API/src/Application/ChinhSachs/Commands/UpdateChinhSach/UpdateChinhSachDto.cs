using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.ChinhSachs.Commands.UpdateChinhSach;
public class UpdateChinhSachDto
{
    public int Id { get; set; }
    public string? Ten { get; set; }
    public string? Mota { get; set; }
    public float? PhanTramGiam { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
public class LichHocDto
{
    public int Thu { get; init; }
    public int PhongId { get; init; }
    public TimeOnly GioBatDau { get; init; } 
    public TimeOnly GioKetThuc { get; init; } 
}

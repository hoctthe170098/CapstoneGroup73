using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.LichHocs.Commands.UpdateLichDayBu
{
    public class LichDayBuDto
    {
        public required DateOnly NgayHocBu {  get; set; }
        public required int PhongId { get; set; }
        public required string GioBatDau { get;set; }
        public required string GioKetThuc { get; set; }
    }
}

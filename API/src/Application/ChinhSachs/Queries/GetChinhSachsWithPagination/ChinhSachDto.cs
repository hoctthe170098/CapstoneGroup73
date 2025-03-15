using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChinhSachs.Queries.GetChinhSachsWithPagination;
public class ChinhSachDto
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string Mota { get; set; } = string.Empty;
    public float PhanTramGiam { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChinhSach, ChinhSachDto>();
            
        }
    }
}

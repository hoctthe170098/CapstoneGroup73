using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChinhSachs.Queries.GetAllChinhSachs;
public class ChinhSachDto
{
    public int Id { get; set; }
    public required string Ten { get; set; }
    public required string Mota { get; set; }
    public required float PhanTramGiam { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChinhSach, ChinhSachDto>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.GiaoViens.Queries.GetGiaoViensByNameOrCode;
public class GiaoVienDto
{
    public required string Code { get; set; }
    public required string CodeTen { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GiaoVien, GiaoVienDto>()
                .ForMember(dest => dest.CodeTen, opt => opt.MapFrom(src => src.Code+" - "+src.Ten));   
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.HocSinhs.Queries.GetHocSinhsByNameOrCode;
public class HocSinhDto
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public required string Email { get; set; }
    public required string SoDienThoai { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<HocSinh, HocSinhDto>();   
        }
    }
}


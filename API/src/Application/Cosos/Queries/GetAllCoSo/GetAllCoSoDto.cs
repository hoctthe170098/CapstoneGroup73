using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Queries.GetAllCoSo;
public class GetAllCoSoDto
{
    public Guid Id { get; init; }
    public string? Ten { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CoSo, GetAllCoSoDto>();
        }
    }
}

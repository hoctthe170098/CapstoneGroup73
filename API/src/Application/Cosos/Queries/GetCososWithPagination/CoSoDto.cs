using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
public class CoSoDto
{
    public Guid Id { get; init; }
    public string? Ten { get; init; }
    public string? DiaChi { get; init; }
    public string? SoDienThoai { get; init; }
    public string? TrangThai { get; init; }
    public bool? Default { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CoSo, CoSoDto>();
        }
    }
}

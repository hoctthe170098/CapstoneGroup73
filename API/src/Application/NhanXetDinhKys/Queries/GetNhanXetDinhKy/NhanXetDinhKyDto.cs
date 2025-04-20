using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
public class NhanXetDinhKyDto
{
    public Guid Id { get; set; }
    public int STT { get; set; }
    public DateOnly NgayNhanXet { get; set; }
    public required string NoiDungNhanXet { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<NhanXetDinhKy, NhanXetDinhKyDto>();
        }
    }
}

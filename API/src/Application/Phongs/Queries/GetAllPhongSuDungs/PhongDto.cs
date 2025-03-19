using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Phongs.Queries.GetAllPhongSuDungs;

public class PhongDto
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Phong, PhongDto>();
        }
    }
}

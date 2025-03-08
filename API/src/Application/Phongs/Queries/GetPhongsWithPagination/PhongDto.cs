using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Phongs.Queries.GetPhongsWithPagination;

public class PhongDto
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public Guid CoSoId { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Phong, PhongDto>();
        }
    }
}

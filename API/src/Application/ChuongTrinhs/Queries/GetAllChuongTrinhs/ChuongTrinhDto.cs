using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetAllChuongTrinhs;

public class ChuongTrinhDto
{
    public int Id { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChuongTrinh, ChuongTrinhDto>();
        }
    }
}

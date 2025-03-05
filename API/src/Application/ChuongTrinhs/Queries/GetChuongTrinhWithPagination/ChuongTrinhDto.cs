using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;

public class ChuongTrinhDto
{
    public int Id { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string MoTa { get; init; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChuongTrinh, ChuongTrinhDto>();
        }
    }
}

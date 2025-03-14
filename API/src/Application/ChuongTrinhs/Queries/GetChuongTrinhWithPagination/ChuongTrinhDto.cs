using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;

public class ChuongTrinhDto
{
    public int Id { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string MoTa { get; init; } = string.Empty;
    public List<NoiDungBaiHocDto> NoiDungBaiHocs { get; init; } = new List<NoiDungBaiHocDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChuongTrinh, ChuongTrinhDto>();
            CreateMap<NoiDungBaiHoc, NoiDungBaiHocDto>();
            CreateMap<TaiLieuHocTap, TaiLieuHocTapDto>();
        }
    }
}

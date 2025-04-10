using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.BaiKiemTras.Queries.GetLichKiemTraChoGiaoVien;
public class BaiKiemTraDto
{
    public Guid Id { get; set; }
    public required string Ten { get; set; }
    public required string UrlFile { get; set; }
    public required DateOnly NgayKiemTra { get; set; }
    public required string TrangThai { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BaiKiemTra, BaiKiemTraDto>();
        }
    }
}


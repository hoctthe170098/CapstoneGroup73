using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
public class LopHocWithPaginationDTO
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public List<LopHocDto> LopHocs { get; set; } = new List<LopHocDto>();
}
public class LopHocDto
{
    public string TenGiaoVien { get; set; } = "";
    public string TenLop { get; set; } = "";
    public string TenChuongTrinh { get; set; } = "";
    public int HocPhi { get; set; }
    public List<LoaiLichHocDto> LoaiLichHocs { get; set; } = new List<LoaiLichHocDto>();
}
public class LoaiLichHocDto
{
    public string TrangThai { get; set; } = "";
    public List<LichHocDto> LichHocs { get; set; } = new List<LichHocDto>();
}
public class LichHocDto
{
    public Guid Id { get; set; }
    public int Thu { get; set; }
    public TimeOnly gioBatDau { get; set; }
    public TimeOnly gioKetThuc { get; set; }
    public string TenPhong { get; set; } = "";
    public DateOnly ngayBatDau { get; set; }
    public DateOnly ngayKetThuc { get; set; }
}

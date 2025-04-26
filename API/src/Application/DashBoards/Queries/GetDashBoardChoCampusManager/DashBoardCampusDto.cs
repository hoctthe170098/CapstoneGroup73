using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DashBoards.Queries.GetDashBoardChoCampusManager;
public class DashBoardCampusDto
{
    public int SoHocSinh {  get; set; }
    public int SoGiaoVien { get; set; }
    public int SoLopHoc { get; set; }
    public required List<ChinhSachDto> HocSinhTheoChinhSachs {  get; set; }
    public int TiLeDiemDanh { get; set; }
    public required List<SoLopHocTrongThangDto> SoLopHoc6ThangGanNhat {  get; set; }
    public required List<DiemDanhTheoLopDto> DiemDanhTheoLops { get; set; }
    public required List<PhongHocDto> ThoiGianSuDungPhongHocs { get; set; }
}
public class SoLopHocTrongThangDto
{
    public int Thang {  get; set; }
    public int Nam {  get; set; }
    public int SoLopHoc { get;set; }
}
public class ChinhSachDto
{
    public required string TenChinhSach { get; set; }
    public int SoHocSinh { get; set; }
}
public class DiemDanhTheoLopDto
{
    public required string TenLop {  get; set; }
    public int SoBuoiHoc { get; set; }
    public int TiLeDiemDanh { get; set; }
    public List<HocSinhNghiNhieuNhatDto> HocSinhNghiNhieuNhats { get; set; } = new List<HocSinhNghiNhieuNhatDto>();
}
public class PhongHocDto
{
    public required string TenPhong {  get; set; }
    public int PhanTramThoiGianSuDungPhong {  get; set; }
}
public class HocSinhNghiNhieuNhatDto
{
    public required string HocSinhCode {  get; set; }
    public required string TenHocSinh { get; set; }
    public int SoBuoiNghi { get;set; }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DashBoards.Queries.GetDashBoardChoAdmin;
public class DashBoardAdminDto
{
    public int SoHocSinh {  get; set; }
    public int SoGiaoVien { get; set; }
    public int SoNhanVien { get; set; }
    public required List<CoSoDto> HocSinhGiaoVienLopHocTheoCoSos { get; set; }
    public required List<ChinhSachDto> HocSinhTheoChinhSachs {  get; set; }
    public int SoLopHoc {  get; set; }
    public int SoLopHocDangDiemRa {  get; set; }
    public int TongSoBuoiHoc {  get; set; }
    public int TongSoBuoiNghi { get; set; }
}
public class CoSoDto
{
    public required string TenCoSo { get; set; }
    public int SoHocSinh { get; set; }
    public int SoGiaoVien { get; set; }
    public int SoLopHoc { get; set; }
}
public class ChinhSachDto
{
    public required string TenChinhSach { get; set; }
    public int SoHocSinh { get; set; }
}


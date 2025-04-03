export class LopDangDay {
  id: string = '';               
  thu: number = 0;               
  phongId: number = 0;           
  tenLop: string = '';           
  gioBatDau: string = '';        
  gioKetThuc: string = '';
  ngayBatDau: string = '';       
  ngayKetThuc: string = '';
  hocPhi: number = 0;            
  trangThai: string = '';        
  giaoVienCode: string = '';     
  chuongTrinhId: number = 0;
}

export class DanhSachHocSinh {
  code: string = '';
  ten: string = '';
  gioiTinh: string = '';
  diaChi: string = '';
  lop: string = '';
  truongDangHoc: string = '';
  ngaySinh: Date = new Date();
  email?: string;
  soDienThoai?: string;
}
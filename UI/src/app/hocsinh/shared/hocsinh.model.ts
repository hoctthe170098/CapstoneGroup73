import { CoSo } from '../../coso/shared/coso.model';

export class HocSinh {
  code: string = '';
  ten: string = '';
  gioiTinh: string = '';
  diaChi: string = '';
  lop: string = '';
  truongDangHoc: string = '';
  ngaySinh: Date = new Date();
  email?: string;
  soDienThoai?: string;
  coSoId: string = '';
  coso: CoSo = new CoSo();
  // Thay chinhSachId thành chinhSach (tên chính sách, ví dụ "Cơ bản")
  chinhSach: string = '';
  // Danh sách các lớp học mà học sinh tham gia
  lopHocs: string[] = [];
  
  province?: string;  // Mã hoặc tên tỉnh/thành
  district?: string;  // Mã hoặc tên quận/huyện
}

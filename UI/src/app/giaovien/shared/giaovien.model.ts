import { CoSo } from '../../coso/shared/coso.model';

export class Giaovien {
  code: string = '';
  ten: string = '';
  gioiTinh: string = '';
  diaChi: string = '';
  lop: string = '';
  truongDangDay: string = '';
  ngaySinh: Date = new Date();
  email?: string;
  soDienThoai?: string;
  coSoId: string = '';
  coso: CoSo = new CoSo();
 
  lopHocs: string[] = [];
  
  province?: string;  // Mã hoặc tên tỉnh/thành
  district?: string;  // Mã hoặc tên quận/huyện
}

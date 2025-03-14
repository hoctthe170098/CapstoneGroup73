import { CoSo } from '../../coso/shared/coso.model';

export class Accountmanager {
  code: string = '';
  ten: string = '';
  gioiTinh: string = '';
  diaChi: string = '';
 
  ngaySinh: Date = new Date();
  email?: string;
  soDienThoai?: string;
  coSoId: string = '';
  coso: CoSo = new CoSo();
  role: string ='CampusManager';
 
  province?: string;  // Mã hoặc tên tỉnh/thành
  district?: string;  // Mã hoặc tên quận/huyện
}

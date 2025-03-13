
export class LopHoc {
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
  
    phong?: any;        
    giaoVien?: any;     
    chuongTrinh?: any; 
    
    thamGiaLopHocs: any[] = [];
    baiTaps: any[] = [];
    baiKiemTras: any[] = [];
  }
  
export interface LopHoc {
    tenLop: string;
    tenChuongTrinh: string;
    tenPhong: string;
    gioBatDau: string;
    gioKetThuc: string;
    trangThai: string;
  }
  
  export interface LichHocTrongNgay {
    thu: number;
    ngay: string; // Format: yyyy-MM-dd
    lops: LopHoc[];
  }
  
  export interface GetLichHocGiaoVienResponse {
    isError: boolean;
    code: number;
    message: string;
    data: {
      tuan: number;
      nam: number;
      lichHocCaTuans: LichHocTrongNgay[];
    };
  }
  
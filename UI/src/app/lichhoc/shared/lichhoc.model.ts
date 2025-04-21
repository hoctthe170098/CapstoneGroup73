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
  ngay: string;
  lops: LopHoc[];
}

export interface GetLichHocHocSinhResponse {
  isError: boolean;
  code: number;
  message: string;
  data: {
    tuan: number;
    nam: number;
    lichHocCaTuans: LichHocTrongNgay[];
  };
}

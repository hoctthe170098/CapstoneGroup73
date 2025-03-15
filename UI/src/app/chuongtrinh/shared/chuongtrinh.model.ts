export interface CreateChuongTrinh {
    id: number;
    tieuDe: string;
    moTa: string;
    noiDungBaiHocs?: CreateNoiDungBaiHoc[];
}

export interface CreateNoiDungBaiHoc {
    tieuDe: string;
    mota: string;
    soThuTu: number;
    taiLieuHocTaps?: CreateTaiLieuHocTap[];
}

export interface CreateTaiLieuHocTap {
    urlType: string;
    file?: File;
}


import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "environments/environment";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
@Injectable({
  providedIn: "root",
})
export class LopdangdayService {
  private baseUrl = `${environment.apiURL}`;
  constructor(private http: HttpClient) {}

  getDanhSachLopHoc(
    pageNumber: number,
    pageSize: number,
    searchClass: string,
    startDate: string,
    endDate: string
  ): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const body = {
      pageNumber,
      pageSize,
      searchClass,
      startDate,
      endDate,
    };

    return this.http.post<any>(
      `${this.baseUrl}/GiaoViens/getgiaovienassignedclass`,
      body,
      { headers }
    );
  }

  getDanhSachHocSinhLop(tenLop: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const body = {
      tenLop,
    };

    return this.http.post<any>(
      `${this.baseUrl}/ThamGiaLopHocs/gethocsinhsinclass`,
      body,
      { headers }
    );
  }
  getBaiTapsForTeacher(payload: {
    pageNumber: number;
    pageSize: number;
    tenLop: string;
    trangThai: string;
  }): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    return this.http.post<any>(
      `${this.baseUrl}/BaiTaps/getbaitapsforteacher`,
      payload,
      { headers }
    );
  }
  deleteBaiTap(id: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
    return this.http.delete<any>(
      `${this.baseUrl}/BaiTaps/deletebaitap?id=${id}`,
      { headers }
    );
  }
  createBaiTap(formData: FormData): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.post(`${this.baseUrl}/BaiTaps/createbaitap`, formData, {
      headers,
    });
  }
  getBaiTapDetail(baiTapId: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
    const url = `${this.baseUrl}/BaiTaps/getbaitapdetailforteacher?baiTapId=${baiTapId}`;
    return this.http.post<any>(url, {}, { headers });
  }
  downloadBaiTapFile(filePath: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    });
    return this.http.post<any>(
      `${this.baseUrl}/BaiTaps/downloadbaitap?filePath=${encodeURIComponent(
        filePath
      )}`,
      {},
      { headers }
    );
  }
  updateBaiTap(formData: FormData): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.put<any>(
      `${this.baseUrl}/BaiTaps/updatebaitap`,
      formData,
      { headers }
    );
  }

  getDiemDanhTheoNgay(tenLop: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const url = `${
      this.baseUrl
    }/DiemDanhs/getdiemdanhtheongay?TenLop=${encodeURIComponent(tenLop)}`;
    return this.http.get<any>(url, { headers });
  }
  updateDiemDanhTheoNgay(payload: any): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`)
      .set("Content-Type", "application/json");

    const url = `${this.baseUrl}/DiemDanhs/updatediemdanhtheongay`;
    return this.http.post<any>(url, payload, { headers });
  }
  getBaoCaoDiemDanh(tenLop: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
    const url = `${
      this.baseUrl
    }/DiemDanhs/getbaocaodiemdanh?TenLop=${encodeURIComponent(tenLop)}`;
    return this.http.get<any>(url, { headers });
  }
  getLichKiemTraChoGiaoVien(tenLop: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const url = `${
      this.baseUrl
    }/BaiKiemTras/getlichkiemtrachogiaovien?TenLop=${encodeURIComponent(
      tenLop
    )}`;
    return this.http.get<any>(url, { headers });
  }
  getKetQuaBaiKiemTraChoGiaoVien(baiKiemTraId: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const url = `${this.baseUrl}/KetQuaBaiKiemTras/getdiembaikiemtrachogiaovien?BaiKiemTraId=${baiKiemTraId}`;
    return this.http.get<any>(url, { headers });
  }
  downloadBaiKiemTra(filePath: string): Observable<Blob> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    });

    const body = { filePath };

    return this.http.post(
      `${this.baseUrl}/BaiKiemTras/downloadbaikiemtra`,
      body,
      {
        headers,
        responseType: "blob",
      }
    );
  }

  getTraLoiByBaiTapForTeacher(payload: {
    baiTapId: string;
    pageNumber: number;
    pageSize: number;
  }): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    return this.http.post<any>(
      `${this.baseUrl}/TraLois/gettraloibybaitapforteacher`,
      payload,
      { headers }
    );
  }
  updateKetQuaBaiKiemTra(payload: any): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`)
      .set("Content-Type", "application/json");

    const url = `${this.baseUrl}/KetQuaBaiKiemTras/updateketquabaikiemtra`;
    return this.http.post<any>(url, payload, { headers });
  }
  getBaoCaoDiemHangNgay(tenLop: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
    const url = `${
      this.baseUrl
    }/DiemDanhs/getbaocaodiemhangngaycuahocsinh?TenLop=${encodeURIComponent(
      tenLop
    )}`;
    return this.http.get<any>(url, { headers });
  }
  updateNhanXetVaDiem(payload: {
    traLoiId: string;
    diem: number;
    nhanXet: string;
  }): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    return this.http.post<any>(
      `${this.baseUrl}/TraLois/updatenhanxet`,
      payload,
      { headers }
    );
  }

  getChuongTrinhLopHoc(tenLop: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    const body = {
      tenLop
    };

    return this.http.post<any>(
      `${this.baseUrl}/ChuongTrinhs/getchuongtrinhbyclass`,
      body,
      { headers }
    );
  }
  getNhanXetDinhKy(tenLop: string, hocSinhCode: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
    const url = `${this.baseUrl}/NhanXetDinhKys/getnhanxetdinhky?TenLop=${encodeURIComponent(tenLop)}&HocSinhCode=${encodeURIComponent(hocSinhCode)}`;
    return this.http.get<any>(url, { headers });
  }
  createNhanXetDinhKy(payload: {
    hocSinhCode: string;
    tenLop: string;
    noiDungNhanXet: string;
  }): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(
      `${this.baseUrl}/NhanXetDinhKys/createnhanxetdinhky`,
      payload,
      { headers }
    );
  }
  updateNhanXetDinhKy(payload: {
    id: string;
    noiDungNhanXet: string;
  }): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`)
      .set("Content-Type", "application/json");
  
    return this.http.put<any>(
      `${this.baseUrl}/NhanXetDinhKys/updatenhanxetdinhky`,
      payload,
      { headers }
    );
  }
  deleteNhanXetDinhKy(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.delete<any>(
      `${this.baseUrl}/NhanXetDinhKys/deletenhanxetdinhky?id=${id}`,
      { headers }
    );
  }
}

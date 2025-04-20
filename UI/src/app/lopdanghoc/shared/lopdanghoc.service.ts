import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class LopdanghocService {
  private baseUrl = `${environment.apiURL}`;
  constructor(private http: HttpClient) {}

  getDanhSachLopHoc(
    pageNumber: number,
    pageSize: number,
    searchClass: string
  ): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const body = {
      pageNumber,
      pageSize,
      searchClass,
    };

    return this.http.post<any>(
      `${this.baseUrl}/ThamGiaLopHocs/gethocsinhassignedclass`,
      body,
      { headers }
    );
  }
  getBaiTapsForStudent(payload: {
    pageNumber: number;
    pageSize: number;
    trangThai: string;
  }): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    return this.http.post<any>(
      `${this.baseUrl}/BaiTaps/getbaitapsforstudent`,
      payload,
      { headers }
    );
  }
  getBaiTapDetailForStudent(baiTapId: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    return this.http.post<any>(
      `${this.baseUrl}/BaiTaps/getbaitapdetailforstudent?BaiTapId=${baiTapId}`,
      {},
      { headers }
    );
  }
  downloadBaiTapFile(filePath: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
  
    return this.http.post<any>(
      `${this.baseUrl}/BaiTaps/downloadbaitap?filePath=${encodeURIComponent(filePath)}`, 
      {},
      { headers }
    );
  }
  
  createTraLoi(formData: FormData): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<any>(`${this.baseUrl}/TraLois/create`, formData, {
      headers,
    });
  }
  getTraLoiByBaiTapForStudent(baiTapId: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.get<any>(
      `${this.baseUrl}/TraLois/gettraloibybaitapforstudent?BaiTapId=${baiTapId}`,
      { headers }
    );
  }
  deleteTraLoi(traLoiId: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.delete<any>(
      `${this.baseUrl}/TraLois/delete?traLoiId=${traLoiId}`,
      { headers }
    );
  }
  updateTraLoi(formData: FormData): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  
    return this.http.put<any>(
      `${this.baseUrl}/TraLois/update`,
      formData,
      { headers }
    );
  }
  getBaoCaoTatCaCacDiem(tenLop: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.get<any>(
      `${this.baseUrl}/DiemDanhs/getbaocaotatcacacdiem?TenLop=${encodeURIComponent(tenLop)}`,
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
  
}

import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "environments/environment";
import { Observable } from "rxjs";
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class LopdangdayService {
  private baseUrl = `${environment.apiURL}`;
  constructor(private http: HttpClient) {}

  getDanhSachLopHoc(pageNumber: number, pageSize: number, searchClass: string, startDate: string, endDate: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`);
     
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
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`);
     
    const body = {
      tenLop
    };

    return this.http.post<any>(
      `${this.baseUrl}/ThamGiaLopHocs/gethocsinhsinclass`,
      body,
      { headers }
    );
  }
  getBaiTapsForTeacher(payload: { pageNumber: number, pageSize: number, tenLop: string, trangThai: string }): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    return this.http.post<any>(
      `${this.baseUrl}/BaiTaps/getbaitapsforteacher`,
      payload,
      { headers }
    );
  }
  deleteBaiTap(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<any>(`${this.baseUrl}/BaiTaps/deletebaitap?id=${id}`, { headers });
  }
  createBaiTap(formData: FormData): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  
    return this.http.post(
      `${this.baseUrl}/BaiTaps/createbaitap`,
      formData,
      { headers }
    );
  }
  
}

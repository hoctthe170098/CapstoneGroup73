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
  getDiemDanhTheoNgay(tenLop: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`);
  
    const url = `${this.baseUrl}/DiemDanhs/getdiemdanhtheongay?TenLop=${encodeURIComponent(tenLop)}`;
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
    const url = `${this.baseUrl}/DiemDanhs/getbaocaodiemdanh?TenLop=${encodeURIComponent(tenLop)}`;
    return this.http.get<any>(url, { headers });
  }
  
}

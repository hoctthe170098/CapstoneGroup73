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

  getDanhSachHocSinhLop(id: string): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`);
     
    const body = {
      id
    };

    return this.http.post<any>(
      `${this.baseUrl}/ThamGiaLopHocs/gethocsinhsinclass`,
      body,
      { headers }
    );
  }
}

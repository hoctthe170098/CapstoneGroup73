import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
@Injectable({
  providedIn: 'root'
})
export class LophocService {
  private phongUrl = `${environment.apiURL}/Phongs`;
  private giaovienUrl = `${environment.apiURL}/Giaoviens`;
  private baseUrl = `${environment.apiURL}/LichHocs`;
  private chuongtrinhUrl = `${environment.apiURL}/ChuongTrinhs`;

  constructor(private http: HttpClient) {}

  getPhongs(): Observable<any> {
    const token = localStorage.getItem('token'); 
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable((observer) => observer.error('Unauthorized: Token missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.phongUrl}/getallphongsudungduoc`, { headers });
  }

  getGiaoViens(payload: any): Observable<any> {
    const token = localStorage.getItem('token'); 
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable((observer) => observer.error('Unauthorized: Token missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.giaovienUrl}/getgiaovienbycodeorname`, payload, { headers });
  }
  editLichHoc(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable((observer) => observer.error('Unauthorized: Token missing'));
    }
  
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');
  
    return this.http.put<any>(`${this.baseUrl}/editlichhoc`, payload, { headers });
  }
  

  getDanhSachLopHoc(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');

    return this.http.post<any>(`${this.baseUrl}/getlophocwithpagination`, payload, { headers });
  }
  createLichHocCoDinh(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${environment.apiURL}/LichHocs/createlichhoccodinh`, payload, { headers });
  }
  getChuongTrinhs(): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.chuongtrinhUrl}/getallchuongtrinhs`, { headers });
  }
  getLopHocByTenLop(tenLop: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const url = `${this.baseUrl}/getlophocbyten?TenLop=${encodeURIComponent(tenLop)}`;
    return this.http.get<any>(url, { headers });
  }
  createLichDayThay(payload: { tenLop: string; ngayDay: string; giaoVienCode: string }): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.baseUrl}/createlichdaythay`, payload, { headers });
  }
  updateLichDayThay(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');
  
    return this.http.put<any>(
      `${this.baseUrl}/updatelichdaythay`,
      payload,
      { headers }
    );
  }
  createLichDayBu(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');
  
    return this.http.post(`${environment.apiURL}/LichHocs/createlichdaybu`, payload, { headers });
  }
}

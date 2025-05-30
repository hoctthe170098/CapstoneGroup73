import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { HttpParams } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class LophocService {
  private phongUrl = `${environment.apiURL}/Phongs`;
  private giaovienUrl = `${environment.apiURL}/Giaoviens`;
  private baseUrl = `${environment.apiURL}/LichHocs`;
  private chuongtrinhUrl = `${environment.apiURL}/ChuongTrinhs`;
  private hocSinhUrl = `${environment.apiURL}/HocSinhs`;
  private diemdanhUrrl = `${environment.apiURL}/DiemDanhs`;
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
 
    return this.http.put<any>(`${this.baseUrl}/editlichhoccodinh`, payload, { headers });
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
  deleteLichDayThay(lichHocId: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.delete(`${environment.apiURL}/LichHocs/deletelichdaythay?lichHocId=${lichHocId}`, {
      headers
    });
  }

  searchHocSinh(payload: { searchTen: string }): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.hocSinhUrl}/gethocsinhbycodeorname`,payload,{ headers });
  }
  deleteLichDayBu(lichHocId: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const url = `${this.baseUrl}/deletelichdaybu?lichHocId=${lichHocId}`;
    return this.http.delete<any>(url, { headers });
  }  
  updateLichDayBu(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');
  
    return this.http.put<any>(`${this.baseUrl}/updatelichdaybu`, payload, { headers });
  }
  deleteLopHocCoDinh(tenLopHoc: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const url = `${this.baseUrl}/deletelophoccodinh?tenLopHoc=${encodeURIComponent(tenLopHoc)}`;
    return this.http.delete<any>(url, { headers });
  }
  getBaoCaoDiemDanh(tenLop: string, ngay?: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    let params = new HttpParams().set('TenLop', tenLop);
    if (ngay) {
      params = params.set('Ngay', ngay);
    }
  
    return this.http.get<any>(`${this.diemdanhUrrl}/getbaocaodiemdanhchotunglop`, { params, headers });
  }
  
  updateDiemDanh(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${environment.apiURL}/DiemDanhs/updatediemdanh`, payload, { headers });
  }
  getBaoCaoHocPhi(tenLop: string, thang?: number, nam?: number) {
    const token = localStorage.getItem('token'); // đảm bảo bạn đã login và lưu token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    let params = new HttpParams().set('TenLop', tenLop);
    if (thang !== undefined) {
      params = params.set('Thang', thang.toString());
    }
    if (nam !== undefined) {
      params = params.set('Nam', nam.toString());
    }
    return this.http.get<any>(`${this.diemdanhUrrl}/getbaocaohocphi`, { params, headers });
  }
}

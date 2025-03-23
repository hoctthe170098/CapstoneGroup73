import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
@Injectable({
  providedIn: 'root'
})
export class PhongService {
  private phongUrl = `${environment.apiURL}/Phongs`;
  private giaovienUrl = `${environment.apiURL}/Giaoviens`;
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
  createLichHocCoDinh(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${environment.apiURL}/LichHocs/createlichhoccodinh`, payload, { headers });
  }
  getChuongTrinhs(): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable((observer) => observer.error('Unauthorized: Token missing'));
    }
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.chuongtrinhUrl}/getallchuongtrinhs`, { headers });
  }
  
  
}

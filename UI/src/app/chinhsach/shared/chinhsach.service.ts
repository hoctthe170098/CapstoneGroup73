import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';
import { ChinhSach } from './chinhsach.model';

@Injectable({
  providedIn: 'root'
})
export class ChinhSachService {
  private baseUrl = `${environment.apiURL}/ChinhSachs`;

  constructor(private http: HttpClient) {}

  getDanhSachChinhSach(): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = {}; 
    return this.http.get<any>(`${this.baseUrl}/getallchinhsachs`, { headers });
  }
  getChinhSachs(payload: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.baseUrl}/getchinhsachs`, payload, { headers });
  }
  createChinhSach(chinhSach: {
    ten: string;
    mota: string;
    phanTramGiam: number;
  }): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    
    const body = {
      createChinhSachDto: {
        ten: chinhSach.ten,
        mota: chinhSach.mota,
        phanTramGiam: chinhSach.phanTramGiam
      }
    };
  
    return this.http.post<any>(`${this.baseUrl}/createchinhsach`, body, { headers });
  }
  deleteChinhSach(id: number): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    
    return this.http.delete<any>(`${this.baseUrl}/deletechinhsach/${id}`, { headers });
  }
  updateChinhSach(chinhSach: {
    id: number;
    ten: string;
    mota: string;
    phanTramGiam: number;
  }): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    const body = {
      updateChinhSachDto: {
        id: chinhSach.id,
        ten: chinhSach.ten,
        mota: chinhSach.mota,
        phanTramGiam: chinhSach.phanTramGiam
      }
    };
  
    return this.http.put<any>(`${this.baseUrl}/updatechinhsach`, body, { headers });
  }
}

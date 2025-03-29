import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaiKiemTraDto } from './testlist.model';
@Injectable({ providedIn: 'root' })
export class TestlistService {
  private apiUrl = 'https://localhost:5001/api/BaiKiemTras/getbaikiemtraswithpagination';
  private createUrl = 'https://localhost:5001/api/BaiKiemTras/createbaikiemtra';

  constructor(private http: HttpClient) {}

  getTests(pageNumber: number, pageSize: number, trangThai: string, tenLop: string, tenBaiKiemTra: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable(observer => observer.error('Unauthorized: Token missing'));
    }
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    const body = {
      pageNumber,
      pageSize,
      trangThai,
      tenLop,
      tenBaiKiemTra
    };
  
    return this.http.post<any>(this.apiUrl, body, { headers });
  }
  createTest(formData: FormData): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.post(this.createUrl, formData, {
      headers,
    });
  }
  
  
  
  getLopByName(tenLop: string): Observable<string[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable(observer => observer.error('Unauthorized: Token missing'));
    }
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const url = `https://localhost:5001/api/LichHocs/gettenlophocbyname?TenLop=${encodeURIComponent(tenLop)}`;
  
    return this.http.get<string[]>(url, { headers });
  }
}

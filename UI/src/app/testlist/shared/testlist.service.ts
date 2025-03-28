import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TestlistService {
  private apiUrl = 'https://localhost:5001/api/BaiKiemTras/getbaikiemtraswithpagination';

  constructor(private http: HttpClient) {}

  getTests(pageNumber: number, pageSize: number, trangThai: string, tenLop: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable((observer) => observer.error('Unauthorized: Token missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const body = {
      pageNumber,
      pageSize,
      trangThai,
      tenLop
    };

    return this.http.post<any>(this.apiUrl, body, { headers });
  }
}

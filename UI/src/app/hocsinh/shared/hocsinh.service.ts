import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment'; 
import { HocSinh } from './hocsinh.model';

@Injectable({
  providedIn: 'root'
})
export class HocSinhService {

  private provinceApiUrl = 'https://provinces.open-api.vn/api/?depth=2';
  private baseUrl = `${environment.apiURL}/HocSinhs`;
  private CHINH_SACH_URL = 'https://localhost:5001/api/ChinhSachs';
  constructor(private http: HttpClient) {}


  getProvinces(): Observable<any> {
    return this.http.get<any>(this.provinceApiUrl);
  }
  getDanhSachHocSinh(pageNumber: number, pageSize: number, searchTen: string, sortBy: string, filterIsActive: boolean | null, filterByClass: string): Observable<any> {
    const token = localStorage.getItem('token'); // 🔑 Lấy token từ localStorage (hoặc sessionStorage)
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const body = {
      pageNumber,
      pageSize,
      searchTen,
      sortBy,
      filterIsActive,
      filterByClass
    };
    return this.http.post<any>(`${this.baseUrl}/gethocsinhswithpagination`, body, { headers });
  }

  /** 🟢 Lấy danh sách chính sách (Có Bearer Token) */
  getDanhSachChinhSach(): Observable<any> {
    const token = localStorage.getItem('token'); // 🔑 Lấy token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any>(`${this.CHINH_SACH_URL}/getallchinhsachs`, { headers });
  }
}

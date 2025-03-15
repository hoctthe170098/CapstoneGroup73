import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GiaovienService {
  private cosoUrl = `${environment.apiURL}/CoSos`;
  private baseUrl = `${environment.apiURL}/GiaoViens`;
  
  constructor(private http: HttpClient) {}

  /** 🏢 Lấy danh sách cơ sở (có phân quyền) */
  getDanhSachCoSo(pageNumber: number = 1, pageSize: number = 100, search: string = ''): Observable<any> {
    const token = localStorage.getItem('token'); 
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, search };

    return this.http.post<any>(`${this.cosoUrl}/getcososwithpagination`, body, { headers });
  }

  getDanhSachGiaoVien(pageNumber: number = 1, pageSize: number = 8, searchTen: string = '', sortBy: string = '', isActive: any = null): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    
    const body = { pageNumber, pageSize, searchTen, sortBy, isActive };
  
    console.log("🔍 Gửi request đến API với body:", body); // Debug log
  
    return this.http.post<any>(`${this.baseUrl}/getgiaovienswithpagination`, body, { headers });
  }
  

  /** ➕ Tạo giáo viên mới */
  createGiaoVien(giaoVienData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(`${this.baseUrl}/creategiaovien`, giaoVienData, { headers });
  }

  /** 📝 Chỉnh sửa giáo viên */
  updateGiaoVien(giaoVienData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(`${this.baseUrl}/editgiaovien`, giaoVienData, { headers });
  }
}

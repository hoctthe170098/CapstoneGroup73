import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountmanagerService {
  private provinceApiUrl = 'https://provinces.open-api.vn/api/?depth=2';
  private baseUrl = `${environment.apiURL}/NhanViens`;
  private cosoUrl = `${environment.apiURL}/Cosos`;

  constructor(private http: HttpClient) {}

  getProvinces(): Observable<any> {
    return this.http.get<any>(this.provinceApiUrl);
  }

  getDanhSachNhanVien(pageNumber: number , pageSize: number , searchTen: string = '', sortBy: string = '', isActive: boolean = null
  ): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, searchTen, sortBy, isActive };

    return this.http.post<any>(`${this.baseUrl}/getnhanvienswithpagination`, body, { headers });
  }

  createNhanVien(nhanVienData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');

    return this.http.post<any>(`${this.baseUrl}/createnhanvien`, nhanVienData, { headers });
  }
  getDanhSachCoSo(): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // Tạo URL với query parameters
    const url = `${this.cosoUrl}/getallcosos`;

    // Gửi yêu cầu GET với headers và query parameters
    return this.http.get<any>(url, { headers: headers });
}
updateNhanVien(nhanVienData: any): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders()
    .set('Authorization', `Bearer ${token}`)
    .set('Content-Type', 'application/json');

  return this.http.put<any>(`${this.baseUrl}/editnhanvien`, nhanVienData, { headers });
}

}

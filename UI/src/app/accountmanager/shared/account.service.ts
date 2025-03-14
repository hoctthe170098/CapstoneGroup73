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

  getDanhSachNhanVien(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTen: string = '',
    filterTenCoSo: string = '',
    filterTenVaiTro: string = '',
    sortBy: string = ''
  ): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, searchTen, filterTenCoSo, filterTenVaiTro, sortBy };

    return this.http.post<any>(`${this.baseUrl}/getnhanvienswithpagination`, body, { headers });
  }

  createNhanVien(nhanVienData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');

    return this.http.post<any>(`${this.baseUrl}/createnhanvien`, nhanVienData, { headers });
  }
  getDanhSachCoSo(pageNumber: number = 1, pageSize: number = 100, search: string = ''): Observable<any> {
    const token = localStorage.getItem('token'); // Lấy token từ localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, search };

    return this.http.post<any>(`${this.cosoUrl}/getcososwithpagination`, body, { headers });
}
updateNhanVien(nhanVienData: any): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders()
    .set('Authorization', `Bearer ${token}`)
    .set('Content-Type', 'application/json');

  return this.http.put<any>(`${this.baseUrl}/editnhanvien`, nhanVienData, { headers });
}

}

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountmanagerService {
  private provinceApiUrl = 'https://provinces.open-api.vn/api/?depth=2';
  private baseUrl = `${environment.apiURL}/CoSos`;
  constructor(private http: HttpClient) {}

  getProvinces(): Observable<any> {
    return this.http.get<any>(this.provinceApiUrl);
  }

  getDanhSachCoSo(pageNumber: number = 1, pageSize: number = 100, search: string = ''): Observable<any> {
    const token = localStorage.getItem('token'); // Lấy token từ localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, search };

    return this.http.post<any>(`${this.baseUrl}/getnhanvienswithpagination`, body, { headers });
}

}

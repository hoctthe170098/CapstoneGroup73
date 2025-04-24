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
    const token = localStorage.getItem('token'); //  Lấy token từ localStorage (hoặc sessionStorage)
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

  getDanhSachChinhSach(): Observable<any> {
    const token = localStorage.getItem('token'); //  Lấy token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any>(`${this.CHINH_SACH_URL}/getallchinhsachs`, { headers });
  }
  createHocSinh(hocSinhData: any): Observable<any> {
    const token = localStorage.getItem('token'); //  Lấy token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.baseUrl}/createhocsinh`, hocSinhData, { headers });
  }
  updateHocSinh(hocSinhData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
        .set('Authorization', `Bearer ${token}`)
        .set('Content-Type', 'application/json');

    return this.http.put<any>(`${this.baseUrl}/edithocsinh`, JSON.stringify(hocSinhData), { headers });
}
exportHocSinhsToExcel(hocSinhs: any[]): Observable<Blob> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  const body = {
    hocSinhs: hocSinhs
  };

  return this.http.post(`${this.baseUrl}/exporthocsinhstoexcel`, body, {
    headers,
    responseType: 'blob'
  });
}
importHocSinhsFromExcel(file: File): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders({
    'Authorization': `Bearer ${token}`
  });

  const formData = new FormData();
  formData.append('File', file);

  return this.http.post<any>(`${this.baseUrl}/importhocsinhsfromexcel?file=string`, formData, { headers });
}
addListHocSinhs(body: any): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.post(`${this.baseUrl}/addlisthocsinhs`, body, { headers });
}
getDanhSachLopTheoTen(tenLop: string): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  return this.http.get<any>(
    `https://localhost:5001/api/LichHocs/gettenlophocbyname?TenLop=${encodeURIComponent(tenLop)}`,
    { headers }
  );
}

}

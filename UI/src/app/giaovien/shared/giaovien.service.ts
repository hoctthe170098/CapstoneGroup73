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
  private provinceApiUrl = 'https://provinces.open-api.vn/api/?depth=2';
  
  constructor(private http: HttpClient) {}

  /** 🏢 Lấy danh sách cơ sở (có phân quyền) */
  getDanhSachCoSo(pageNumber: number = 1, pageSize: number = 100, search: string = ''): Observable<any> {
    const token = localStorage.getItem('token'); 
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, search };

    return this.http.post<any>(`${this.cosoUrl}/getcososwithpagination`, body, { headers });
  }

  getProvinces(): Observable<any> {
    return this.http.get<any>(this.provinceApiUrl);
  }

  
  getDanhSachGiaoVien(pageNumber: number = 1, pageSize: number = 8, searchTen: string = '', sortBy: string = '', isActive: any = null): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { pageNumber, pageSize, searchTen, sortBy, isActive };
    return this.http.post<any>(`${this.baseUrl}/getgiaovienswithpagination`, body, { headers });
  }
  

  createGiaoVien(giaoVienData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(`${this.baseUrl}/creategiaovien`, giaoVienData, { headers });
  }

  updateGiaoVien(giaoVienData: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders()
        .set('Authorization', `Bearer ${token}`)
        .set('Content-Type', 'application/json'); 

    return this.http.put<any>(`${this.baseUrl}/editgiaovien`, JSON.stringify(giaoVienData), { headers });
}
exportGiaoViensToExcel(): Observable<Blob> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  return this.http.post(`${this.baseUrl}/exportgiaovienstoexcel`, {}, { headers, responseType: 'blob' });
}
downloadTemplateGiaoViens(): Observable<Blob> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  return this.http.post(`${this.baseUrl}/downloadtemplateexcelgiaovien`, {}, { headers, responseType: 'blob' });
}
importGiaoViensFromExcel(file: File): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders({
    Authorization: `Bearer ${token}`
  });

  const formData = new FormData();
  formData.append('File', file); // key = File (viết hoa theo yêu cầu Postman)

  return this.http.post<any>(
    `${this.baseUrl}/importgiaoviensfromexcel?file=string`,formData,{ headers });
}
addListGiaoViens(giaoViens: any[]): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  const body = { giaoViens };

  return this.http.post<any>(`${this.baseUrl}/addlistgiaoviens`, body, { headers });
}
}

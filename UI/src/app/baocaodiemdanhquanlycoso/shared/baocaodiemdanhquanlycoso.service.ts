import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BaocaodiemdanhquanlycosohService {
  private apiUrl = 'https://localhost:5001/api/DiemDanhs/getbaocaodiemdanhchotunglop';

  constructor(private http: HttpClient) {}

  getBaoCaoDiemDanh(tenLop: string, ngay?: string) {
    const token = localStorage.getItem('token'); // Đảm bảo token đã được lưu trước đó
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    let params = new HttpParams().set('TenLop', tenLop);
    if (ngay) {
      params = params.set('Ngay', ngay);
    }

    return this.http.get<any>(this.apiUrl, { params, headers });
  }
  updateDiemDanh(payload: any) {
    const token = localStorage.getItem('token');
    const headers = {
      Authorization: `Bearer ${token}`
    };
    return this.http.post<any>('https://localhost:5001/api/DiemDanhs/updatediemdanh', payload, { headers });
  }
  
}

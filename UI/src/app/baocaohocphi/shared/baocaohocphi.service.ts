import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BaocaohocphiService {
  private apiUrl = 'https://localhost:5001/api/DiemDanhs/getbaocaohocphi';

  constructor(private http: HttpClient) {}

  getBaoCaoHocPhi(tenLop: string, thang?: number) {
    const token = localStorage.getItem('token'); // đảm bảo bạn đã login và lưu token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    let params = new HttpParams().set('TenLop', tenLop);
    if (thang !== undefined) {
      params = params.set('Thang', thang.toString());
    }

    return this.http.get<any>(this.apiUrl, { params, headers });
  }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GetLichHocGiaoVienResponse } from './lichday.model';
import { environment } from 'environments/environment';
@Injectable({
  providedIn: 'root'
})
export class LichHocService {
  private lichhocUrl = `${environment.apiURL}/Lichhocs`;

  constructor(private http: HttpClient) {}

  getLichHocGiaoVien(tuan?: number, nam?: number): Observable<GetLichHocGiaoVienResponse> {
    let params = new HttpParams();
    if (tuan !== undefined) {
      params = params.set('Tuan', tuan.toString());
    }
    if (nam !== undefined) {
      params = params.set('Nam', nam.toString());
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}` // tùy theo cách bạn lưu token
    });

    return this.http.get<GetLichHocGiaoVienResponse>(`${this.lichhocUrl}/getlichhocgiaovien`, { headers, params });
  }
}

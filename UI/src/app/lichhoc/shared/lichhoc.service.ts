import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { GetLichHocHocSinhResponse } from './lichhoc.model';

@Injectable({
  providedIn: 'root'
})
export class LichHocService {
  private lichhocUrl = `${environment.apiURL}/LichHocs`;

  constructor(private http: HttpClient) {}

  getLichHocHocSinh(tuan?: number, nam?: number): Observable<GetLichHocHocSinhResponse> {
    let params = new HttpParams();
    if (tuan !== undefined) {
      params = params.set('Tuan', tuan.toString());
    }
    if (nam !== undefined) {
      params = params.set('Nam', nam.toString());
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
    });

    return this.http.get<GetLichHocHocSinhResponse>(`${this.lichhocUrl}/getlichhochocsinh`, { headers, params });
  }
}

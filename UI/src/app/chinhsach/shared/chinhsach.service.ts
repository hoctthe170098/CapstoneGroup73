import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';
import { ChinhSach } from './chinhsach.model';

@Injectable({
  providedIn: 'root'
})
export class ChinhSachService {
  private baseUrl = `${environment.apiURL}/ChinhSachs`;

  constructor(private http: HttpClient) {}

  getDanhSachChinhSach(): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = {}; 
    return this.http.get<any>(`${this.baseUrl}/getallchinhsachs`, { headers });
  }
}

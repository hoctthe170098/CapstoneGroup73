import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { Slot } from './slot.model';

@Injectable({
  providedIn: 'root'
})
export class SlotService {

  private baseUrl = `${environment.apiURL}/Phongs`;

  constructor(private http: HttpClient) {}

  getDanhSachPhong(): Observable<any> {
    const token = localStorage.getItem('token'); // Lấy token từ localStorage
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = {}; 
    return this.http.post<any>(`${this.baseUrl}/getphongswithpagination`, body, { headers });
  }

  createPhong(data: Partial<Slot>): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<any>(`${this.baseUrl}/createphong`, data, { headers });
  }

  updatePhong(data: Slot): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put<any>(`${this.baseUrl}/editphong`, data, { headers });
  }
}

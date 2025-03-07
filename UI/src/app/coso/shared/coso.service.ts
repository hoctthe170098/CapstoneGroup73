import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { CoSo } from './coso.model';

@Injectable({
    providedIn: 'root'
})
export class CoSoService {

    private baseUrl = `${environment.apiURL}/CoSos`;

    constructor(private http: HttpClient) {}
    
    getDanhSachCoSo(pageNumber: number, pageSize: number, search: string): Observable<any> {
        const token = localStorage.getItem('token'); // Lấy token từ localStorage
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        const body = {
            pageNumber,
            pageSize,
            search
        };
        return this.http.post<any>(`${this.baseUrl}/getcososwithpagination`, body,{headers});
    }
    createCoSo(data: CoSo): Observable<any> {
        const token = localStorage.getItem('token'); // Lấy token từ localStorage
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        return this.http.post(`${this.baseUrl}/createcoso`, data,{headers});
    }
    updateCoSo(data: CoSo): Observable<any> {
        const token = localStorage.getItem('token'); // Lấy token từ localStorage
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
        return this.http.put(`${this.baseUrl}/editcoso`, data,{headers});
    }
}

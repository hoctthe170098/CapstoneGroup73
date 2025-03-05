import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
        const body = {
            pageNumber,
            pageSize,
            search
        };
        return this.http.post<any>(`${this.baseUrl}/getcososwithpagination`, body);
    }

    createCoSo(data: CoSo): Observable<any> {
        return this.http.post(`${this.baseUrl}/createcoso`, data);
    }

    updateCoSo(data: CoSo): Observable<any> {
        return this.http.put(`${this.baseUrl}/editcoso`, data);
    }
}

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

  constructor(private http: HttpClient) {}


  getProvinces(): Observable<any> {
    return this.http.get<any>(this.provinceApiUrl);
  }
}

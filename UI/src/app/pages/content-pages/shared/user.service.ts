import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangePasswordRequest, ForgotPassword,LoginRequest,LoginResponse } from './user.model';
import { environment } from 'environments/environment';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = environment.apiURL + 'ApplicationUsers';


  constructor(private http: HttpClient) {}


  forgotPassword(request: ForgotPassword): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgotpassword`, request);
  }


  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request);
  }
  changePassword(request: ChangePasswordRequest) {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('token')}`, // Lấy token từ localStorage
      'Content-Type': 'application/json'
    });
  
    return this.http.post(`${this.apiUrl}/changepassword`, request, { headers });
  }
  


 }

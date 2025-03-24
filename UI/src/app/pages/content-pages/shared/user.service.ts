import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangePasswordRequest, ForgotPassword,LoginRequest,LoginResponse } from './user.model';
import { environment } from 'environments/environment';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = environment.apiURL + '/ApplicationUsers';


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
  getToken(): string | null {
    return localStorage.getItem('token');
}

getDecodedToken() {
    const token = this.getToken();
    if (token) {
        try {
            const base64Url = token.split('.')[1];
            let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            switch (base64.length % 4) {
                case 0:
                    break;
                case 2:
                    base64 += '==';
                    break;
                case 3:
                    base64 += '=';
                    break;
                default:
                    throw new Error('Invalid base64');
            }
            const decodedPayload = decodeURIComponent(escape(window.atob(base64)));
            return JSON.parse(decodedPayload);
        } catch (error) {
            console.error('Error decoding token:', error);
            return null;
        }
    }
    return null;
}
// Hàm để lấy role từ token đã giải mã
getRoleNames(): string[] | null {
  const decodedToken = this.getDecodedToken();
  if (decodedToken && decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']) {
      // trả về mảng nếu có nhiều role, ngược lại trả về role string
      if (Array.isArray(decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])){
          return decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      }else{
          return [decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']];
      }
  }
  return null;
}
getUserName(): string | null{
  const decodedToken = this.getDecodedToken();
  if (decodedToken && decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']){
       return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
  }
  return null;
}
}

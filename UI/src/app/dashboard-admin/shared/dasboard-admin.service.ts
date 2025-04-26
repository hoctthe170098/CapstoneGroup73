import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class DashboardAdminService {
  private baseUrl = `${environment.apiURL}`;

  constructor(private http: HttpClient) {}

  getDashboardAdmin(): Observable<any> {
    const token = localStorage.getItem('token') || '';
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any>(
      `${this.baseUrl}/DashBoards/getdashboardadmin`,
      { headers }
    );
  }
}

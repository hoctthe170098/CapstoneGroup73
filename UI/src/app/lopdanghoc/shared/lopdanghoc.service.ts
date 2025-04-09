import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})

export class LopdanghocService {
  private baseUrl = `${environment.apiURL}`;
  constructor(private http: HttpClient) {}

  getDanhSachLopHoc(
    pageNumber: number,
    pageSize: number,
    searchClass: string
    ): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);

    const body = {
      pageNumber,
      pageSize,
      searchClass
    };

    return this.http.post<any>(
      `${this.baseUrl}/ThamGiaLopHocs/gethocsinhassignedclass`,
      body,
      { headers }
    );
  }
}

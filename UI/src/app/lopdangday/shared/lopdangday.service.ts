import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "environments/environment";
import { Observable } from "rxjs";

export class LopdangdayService {
  private baseUrl = `${environment.apiURL}/GiaoViens`;
  constructor(private http: HttpClient) {}

  getDanhSachLopHoc(payload: any): Observable<any> {
    const token = localStorage.getItem("token");
    const headers = new HttpHeaders()
      .set("Authorization", `Bearer ${token}`)
      .set("Content-Type", "application/json");

    return this.http.post<any>(
      `${this.baseUrl}/getgiaovienassignedclass`,
      payload,
      { headers }
    );
  }
}

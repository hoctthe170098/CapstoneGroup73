import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable ,throwError} from 'rxjs';
import { environment } from 'environments/environment'; 
import { catchError, tap, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class TestlistService {
  private baseUrl = `${environment.apiURL}/BaiKiemTras`;
  private ketquaUrl = `${environment.apiURL}/KetQuaBaiKiemTras`; 
  private lopUrl = `${environment.apiURL}/LichHocs`;

  constructor(private http: HttpClient) {}
  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getTests(pageNumber: number, pageSize: number, trangThai: string, tenLop: string, tenBaiKiemTra: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token không tồn tại!');
      return new Observable(observer => observer.error('Unauthorized: Token missing'));
    }
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    const body = {
      pageNumber,
      pageSize,
      trangThai,
      tenLop,
      tenBaiKiemTra
    };
  
    return this.http.post<any>(`${this.baseUrl}/getbaikiemtraswithpagination`, body, { headers });
  }
  createTest(formData: FormData): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.post(`${this.baseUrl}/createbaikiemtra`, formData, { headers });
  }
  updateTest(formData: FormData): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.put(`${this.baseUrl}/updatebaikiemtra`, formData, { headers });
  }
  deleteTest(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.baseUrl}/deletebaikiemtra?id=${id}`, { headers });
  }
  downloadFile(filePath: string): Observable<Blob> {
      const headers = this.getHeaders().set('Content-Type', 'application/json');
      return this.http.post(`${this.baseUrl}/downloadbaikiemtra`, { filePath: filePath }, {
        headers: headers,
        responseType: 'blob' // Yêu cầu response là Blob
      }).pipe(
        catchError(error => {
         
          return throwError(() => error);
        })
      );
    }
  
  getLopByName(tenLop: string): Observable<string[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const url = `${this.lopUrl}/gettenlophocbyname?TenLop=${encodeURIComponent(tenLop)}`;
    return this.http.get<string[]>(url, { headers });
  }
  exportKetquabaikiemtraToExcel(id:string): Observable<Blob> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    return this.http.post(`${this.ketquaUrl}/exportketquabaikiemtra`, {baiKiemTraId:id}, { headers, responseType: 'blob' });
  }
}
  
  
  
  
  

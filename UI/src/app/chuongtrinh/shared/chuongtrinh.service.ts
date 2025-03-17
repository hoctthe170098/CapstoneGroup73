import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { environment } from 'environments/environment';
import { CreateChuongTrinh, CreateNoiDungBaiHoc } from './chuongtrinh.model';

@Injectable({
  providedIn: 'root'
})
export class ChuongtrinhService {
  private apiUrl = `${environment.apiURL}/Chuongtrinhs`;
  private programsSource = new BehaviorSubject<CreateChuongTrinh[]>([]);
  programs$ = this.programsSource.asObservable(); // Observable để component có thể subscribe

  constructor(private http: HttpClient) {}

  /** 🔥 Lấy headers với token để gọi API */
  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  /** 🔥 Lấy danh sách chương trình từ API */
  getPrograms(page: number = 1, search: string = "", pageSize: number = 8): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/getchuongtrinhs`, {
      search: search, // ✅ Thêm từ khóa tìm kiếm
      pageNumber: page,
      pageSize: pageSize
    }, {
      headers: this.getHeaders()
    }).pipe(
      tap(response => {
        console.log(`📌 API Response (Trang ${page}, Tìm kiếm: "${search}")`, response);
      }),
      catchError(error => {
        console.error("❌ Lỗi khi gọi API:", error);
        return throwError(() => error);
      })
    );
  }
  uploadFile(file: File) {
    const formData = new FormData();
    formData.append('file', file);
  
    return this.http.post<{ fileUrl: string }>(
      'https://localhost:5001/api/ChuongTrinhs/uploadfile', 
      formData
    );
  }
  /** 🔥 Lấy danh sách bài học của một chương trình */
  getProgramLessons(id: number): Observable<CreateNoiDungBaiHoc[]> {
    return this.http.get<CreateChuongTrinh>(`${this.apiUrl}/${id}`, {
      headers: this.getHeaders()
    }).pipe(
      tap(program => console.log(`📌 Dữ liệu chương trình ID ${id}:`, program)),
      map(program => program.noiDungBaiHocs || []), // Trả về danh sách bài học (hoặc rỗng)
      catchError(error => {
        console.error(`❌ Lỗi khi lấy danh sách bài học của chương trình ID ${id}:`, error);
        return throwError(() => error);
      })
    );
  }

  /** 🔥 Thêm mới chương trình */
  addProgram(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/createchuongtrinh`, formData, {
      headers: this.getHeaders(), // Không đặt 'Content-Type'
    });
  }
  downloadFile(urlFile: string): Observable<any> {
    const headers = this.getHeaders().set('Content-Type', 'application/json'); // Sử dụng getHeaders() và thêm Content-Type
    return this.http.post(`${this.apiUrl}/downloadtailieuhoctap`, { filePath: urlFile }, {
      headers: headers,
      responseType: 'blob' // Yêu cầu response là Blob
    });
  }
  /** 🔥 Cập nhật chương trình */
  updateProgram(payload: any) {
    return this.http.put(`${this.apiUrl}/updatechuongtrinh`, payload);
  }

  /** 🔥 Xóa chương trình */
  deleteProgram(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/deletechuongtrinh/${id}`, {
      headers: this.getHeaders()
    }).pipe(
      tap(() => {
        const programs = this.programsSource.value.filter(program => program.id !== id);
        this.programsSource.next(programs);
      }),
      catchError(error => {
        console.error(`❌ Lỗi khi xóa chương trình ID ${id}:`, error);
        return throwError(() => error);
      })
    );
  }
}

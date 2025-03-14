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
  private apiUrl = `${environment.apiURL}/ChuongTrinhs`;
  private programsSource = new BehaviorSubject<CreateChuongTrinh[]>([]);
  programs$ = this.programsSource.asObservable(); // Observable để component có thể subscribe

  constructor(private http: HttpClient) {}

  /** 🔥 Lấy headers với token để gọi API */
  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  /** 🔥 Lấy danh sách chương trình từ API */
  getPrograms(): void {
    this.http.get<any>(`${this.apiUrl}/getchuongtrinhs?PageNumber=1&PageSize=10`, {
      headers: this.getHeaders()
    }).subscribe({
      next: (response) => {
        console.log("📌 API Response:", response); // Kiểm tra dữ liệu từ API
        if (response && response.items) {  // ✅ Sửa lỗi lấy dữ liệu
          console.log("✅ Danh sách chương trình học:", response.items);
          this.programsSource.next(response.items); // Cập nhật danh sách
        } else {
          console.warn("⚠️ API không trả về dữ liệu hợp lệ", response);
        }
      },
      error: (error) => {
        console.error("❌ Lỗi khi gọi API:", error);
      }
    });
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

  /** 🔥 Cập nhật chương trình */
  updateProgram(id: number, updatedProgram: CreateChuongTrinh): Observable<CreateChuongTrinh> {
    return this.http.put<CreateChuongTrinh>(`${this.apiUrl}/updatechuongtrinh/${id}`, updatedProgram, {
      headers: this.getHeaders()
    }).pipe(
      tap(() => {
        const programs = this.programsSource.value.map(program =>
          program.id === id ? updatedProgram : program
        );
        this.programsSource.next(programs);
      }),
      catchError(error => {
        console.error(`❌ Lỗi khi cập nhật chương trình ID ${id}:`, error);
        return throwError(() => error);
      })
    );
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

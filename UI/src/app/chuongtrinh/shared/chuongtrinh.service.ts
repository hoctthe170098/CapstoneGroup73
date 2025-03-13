import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChuongtrinhService {

  private programsSource = new BehaviorSubject<any[]>(this.getStoredPrograms());
  programs$ = this.programsSource.asObservable();

  constructor() {}

  // 🔥 Lấy danh sách từ localStorage hoặc dữ liệu mặc định
  private getStoredPrograms(): any[] {
    const storedData = localStorage.getItem('programs');
    if (storedData) {
      const programs = JSON.parse(storedData);
      if (programs.length > 0) {
        return programs;
      }
    }
  
    // Khởi tạo defaultPrograms với mỗi lesson có danh sách file
    const defaultPrograms = [
      {
        title: 'Chương trình học 1',
        description: 'Mô tả chương trình học 1',
        isHidden: false,
        lessons: [
          {
            title: 'Bài 1',
            description: '',
            files: [
              { name: 'sample-lesson-1.pdf', fileUrl: 'assets/files/sample-lesson-1.pdf' }
            ]
          },
          {
            title: 'Bài 2',
            description: '',
            files: [
              { name: 'sample-lesson-2.pdf', fileUrl: 'assets/files/sample-lesson-2.pdf' }
            ]
          }
        ]
      },
      {
        title: 'Chương trình học 2',
        description: 'Mô tả chương trình học 2',
        isHidden: false,
        lessons: [
          {
            title: 'Bài 1',
            description: '',
            files: [
              { name: 'sample-lesson-3.pdf', fileUrl: 'assets/files/sample-lesson-3.pdf' }
            ]
          }
        ]
      }
    ];
  
    localStorage.setItem('programs', JSON.stringify(defaultPrograms));
    return defaultPrograms;
  }
  

  // 🔥 Cập nhật localStorage sau mỗi thay đổi
  private updateLocalStorage(programs: any[]) {
    localStorage.setItem('programs', JSON.stringify(programs));
  }

  // 📌 Lấy danh sách chương trình
  getPrograms(): any[] {
    return this.programsSource.value.slice(); // 🔥 Thay thế spread (...)
  }

  // 📌 Lấy một chương trình theo index
  getProgram(index: number): any {
    return this.programsSource.value[index] ? Object.assign({}, this.programsSource.value[index]) : null;
  }

  // 📌 Cập nhật chương trình
  updateProgram(index: number, updatedProgram: any): void {
    const programs = this.programsSource.value.slice(); // 🔥 Thay thế spread (...)
    if (index >= 0 && index < programs.length) {
      programs[index] = Object.assign({}, updatedProgram); // 🔥 Tránh spread object
      this.programsSource.next(programs);
      this.updateLocalStorage(programs); // 🔥 Lưu lại thay đổi vào localStorage
    }
  }

  // 📌 Thêm chương trình mới
  addProgram(newProgram: any): void {
    const programs = this.programsSource.value.slice(); // 🔥 Thay thế spread (...)
    programs.push(newProgram);
    this.programsSource.next(programs);
    this.updateLocalStorage(programs); // 🔥 Lưu vào localStorage
  }

  // 📌 Xóa chương trình
  deleteProgram(index: number): void {
    const programs = this.programsSource.value.slice(); // 🔥 Thay thế spread (...)
    programs.splice(index, 1);
    this.programsSource.next(programs);
    this.updateLocalStorage(programs); // 🔥 Cập nhật lại localStorage
  }

  // 📌 Giả lập upload file (chưa có backend)
  uploadFile(file: File): Observable<string> {
    const fileName = encodeURIComponent(file.name);
    const fakeServerUrl = `https://example.com/uploads/${fileName}`;
    return of(fakeServerUrl); // Giả lập trả về URL thật từ server
  }
}

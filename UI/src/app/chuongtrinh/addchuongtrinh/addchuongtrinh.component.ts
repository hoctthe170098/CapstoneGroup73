import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';

@Component({
  selector: 'app-addchuongtrinh',
  templateUrl: './addchuongtrinh.component.html',
  styleUrls: ['./addchuongtrinh.component.scss']
})
export class AddchuongtrinhComponent {

  // Khởi tạo đối tượng program mặc định
  program: any = {
    title: '',
    description: '',
    lessons: [
      {
        title: '',
        description: '',
        fileUrl: '',
        file: null
      }
    ]
  };

  constructor(
    private router: Router,
    private chuongtrinhService: ChuongtrinhService
  ) { }

  // Thêm bài học mới
  addLesson() {
    this.program.lessons.push({
      title: '',
      description: '',
      fileUrl: '',
      file: null
    });
  }

  // Xóa bài học
  removeLesson(index: number) {
    this.program.lessons.splice(index, 1);
  }

  // Bắt sự kiện khi người dùng chọn file
  onFileChange(event: Event, index: number) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      // Kiểm tra định dạng file PDF
      if (file.type !== 'application/pdf') {
        alert('Chỉ chấp nhận file PDF!');
        return;
      }
      this.uploadFile(file, index);
    }
  }

  // Upload file thông qua service
  uploadFile(file: File, index: number) {
    this.chuongtrinhService.uploadFile(file).subscribe(
      (fileUrl) => {
        this.program.lessons[index].fileUrl = fileUrl; 
        console.log(`File uploaded for lesson ${index + 1}: ${fileUrl}`);
      },
      (error) => {
        console.error('Lỗi upload file:', error);
        alert('Upload file thất bại!');
      }
    );
  }

  // Lưu chương trình
  saveProgram() {
    console.log('Program to add:', JSON.stringify(this.program, null, 2));
    // Gọi hàm addProgram trong service (bạn cần tự định nghĩa)
    this.chuongtrinhService.addProgram(this.program);
    alert('Thêm chương trình thành công!');
    this.router.navigate(['/chuongtrinh']);
  }

  // Hủy và quay về danh sách
  cancel() {
    this.router.navigate(['/chuongtrinh']);
  }
}

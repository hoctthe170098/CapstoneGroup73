import { Component, OnInit } from '@angular/core';
import { ChuongtrinhService } from './shared/chuongtrinh.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-chuongtrinh',
  templateUrl: './chuongtrinh.component.html',
  styleUrls: ['./chuongtrinh.component.scss']
})
export class ChuongtrinhComponent implements OnInit {

  programs: any[] = [];

  constructor(
    private chuongtrinhService: ChuongtrinhService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.chuongtrinhService.getPrograms(); // 🚀 Gọi API khi vào trang
  
    this.chuongtrinhService.programs$.subscribe(data => {
      if (data && data.length > 0) {
        this.programs = data; // ✅ Cập nhật danh sách
        console.log("📌 Cập nhật danh sách chương trình:", this.programs);
      } else {
        console.warn("⚠️ Không có chương trình nào!");
      }
    });
  }
  
  

  /**
   * Toggle hiển thị nội dung bài học của chương trình
   */
  toggleContent(id: number, event: Event) {
    event.preventDefault();
    const program = this.programs.find(p => p.id === id);
    if (program) {
      program.expanded = !program.expanded;
    }
  }

  /**
   * Tải file đính kèm của bài học
   */
  downloadFile(fileUrl: string) {
    if (!fileUrl) {
      alert('Không có file để tải!');
      return;
    }
    const link = document.createElement('a');
    link.href = fileUrl;
    link.download = fileUrl.substring(fileUrl.lastIndexOf('/') + 1);
    link.click();
  }

  /**
   * Xóa chương trình học theo ID
   */
  // deleteProgram(id: number) {
  //   const confirmDelete = confirm("Bạn có chắc chắn muốn xóa chương trình này?");
  //   if (confirmDelete) {
  //     this.chuongtrinhService.deleteProgram(id);
  //     this.programs = this.chuongtrinhService.getPrograms().map(program => ({ ...program }));
  //     alert('Xóa thành công!');
  //   }
  // }
}

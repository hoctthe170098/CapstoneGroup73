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
    this.chuongtrinhService.programs$.subscribe(data => {
      this.programs = data.slice(); // Lưu danh sách chương trình
    });
  }

  toggleContent(index: number, event: Event) {
    event.preventDefault();
    this.programs[index].expanded = !this.programs[index].expanded;
  }

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

  deleteProgram(index: number) {
    const confirmDelete = confirm(`Bạn có chắc chắn muốn xóa chương trình "${this.programs[index].title}" không?`);
    if (confirmDelete) {
      this.chuongtrinhService.deleteProgram(index);
  
      // 🔥 Đảm bảo Angular nhận diện thay đổi danh sách
      this.programs = this.chuongtrinhService.getPrograms().map(program => Object.assign({}, program));

  
      alert('Xóa thành công!');
    }
  }
  
  
}

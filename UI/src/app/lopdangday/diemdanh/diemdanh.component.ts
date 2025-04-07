import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-diemdanh',
  templateUrl: './diemdanh.component.html',
  styleUrls: ['./diemdanh.component.scss']
})
export class DiemdanhComponent implements OnInit {
  students: any[] = [];

  ngOnInit(): void {
    // Giả lập dữ liệu học sinh
    this.students = Array.from({ length: 9 }, (_, i) => ({
      code: 'HS00001',
      name: 'Bùi Ngọc Dũng',
      diemDanh: '', // 'coMat' hoặc 'vang'
      diemBTVN: '',
      diemTrenLop: '',
      nhanXet: ''
    }));
  }

  onSubmit() {
    console.log('Dữ liệu điểm danh:', this.students);
    // Bạn có thể gọi API để lưu thông tin ở đây
  }
  getDisplayScore(score: string): string {
    return score ? `${score} / 10` : '';
  }
  
  updateScore(value: string, student: any, field: 'diemBTVN' | 'diemTrenLop') {
    // Lấy phần trước dấu "/"
    const trimmed = value.split('/')[0].trim();
    student[field] = trimmed;
  }
}

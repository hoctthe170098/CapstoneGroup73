import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-lophoc',
  templateUrl: './lophoc.component.html',
  styleUrls: ['./lophoc.component.scss']
})
export class LophocComponent implements OnInit {

  // Biến filter
  thuTrongTuan = {
    thu2: false,
    thu3: false,
    thu4: false,
    thu5: false,
    thu6: false,
    thu7: false,
    cn: false
  };
  chuongTrinh: string = '';
  trangThai: string = '';
  timeStart: string = '';
  timeEnd: string = '';
  dateStart: string = '';
  dateEnd: string = '';

  // Danh sách lớp học demo
  lophocs = [
    {
      tenLop: 'Lớp ôn luyện tiếng Anh cho học sinh khối 6',
      chuongTrinh: 'Tiếng Anh nâng cao',
      phong: 'E6-101',
      giaoVien: 'Bùi Ngọc Dũng',
      thoiGian: 'T2 - T6: 7h - 9h, T7: 9h - 11h',
      ngayBatDau: '23/04/2025',
      ngayKetThuc: '30/04/2025',
      hocPhi: '500.000 đồng / tháng'
    },
    {
      tenLop: 'Lớp ôn Toán 9',
      chuongTrinh: 'Toán cấp 2',
      phong: 'E6-202',
      giaoVien: 'Trần Thị B',
      thoiGian: 'T3 - T5: 8h - 10h',
      ngayBatDau: '01/05/2025',
      ngayKetThuc: '31/05/2025',
      hocPhi: '600.000 đồng / tháng'
    }
  ];

  // Phân trang demo
  currentPage: number = 1;
  totalPages: number = 2;

  constructor() { }

  ngOnInit(): void {
  }

  onAddClass() {
    alert('Thêm lớp học');
  }

  onEditClass(index: number) {
    alert('Sửa lớp: ' + this.lophocs[index].tenLop);
  }

  goToPage(page: number) {
    this.currentPage = page;
  }
}

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baocaodiemdanhquanlycoso',
  templateUrl: './baocaodiemdanhquanlycoso.component.html',
  styleUrls: ['./baocaodiemdanhquanlycoso.component.scss']
})
export class BaocaodiemdanhquanlycosoComponent implements OnInit {

  danhSachNgay = [
    'Thứ 3, 15/04/2025',
    'Thứ 2, 14/04/2025',
    'Thứ 7, 13/04/2025',
    'Thứ 6, 12/04/2025',
    'Thứ 5, 11/04/2025',
    'Thứ 4, 10/04/2025'
  ];

  ngayDuocChon = this.danhSachNgay[0];

  danhSachHocSinh = Array.from({ length: 9 }).map(() => ({
    ten: 'Bùi Ngọc Dũng',
    trangThai: 'Có mặt'
  }));

  backupDanhSach: any[] = [];
  dangSua = false;

  constructor() { }

  ngOnInit(): void { }

  chonNgay(date: string) {
    this.ngayDuocChon = date;
    this.dangSua = false;
  }

  huySua() {
    this.danhSachHocSinh = JSON.parse(JSON.stringify(this.backupDanhSach));
    this.dangSua = false;
  }

  luuDiemDanh() {
    console.log('Đã lưu:', this.danhSachHocSinh);
    this.dangSua = false;
  }

  ngOnChanges() {
    if (!this.dangSua) {
      this.backupDanhSach = JSON.parse(JSON.stringify(this.danhSachHocSinh));
    }
  }

}

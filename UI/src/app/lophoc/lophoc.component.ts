import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LophocService } from './shared/lophoc.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-lophoc',
  templateUrl: './lophoc.component.html',
  styleUrls: ['./lophoc.component.scss']
})
export class LophocComponent implements OnInit {

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
  searchTerm: string = '';

  lophocs: any[] = [];

  currentPage: number = 1;
  totalPages: number = 1;
  pageSize: number = 3;

  constructor(
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef, 
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadLopHocs();
  }

  loadLopHocs(page: number = this.currentPage) {
    const thus: number[] = [];
    if (this.thuTrongTuan.thu2) thus.push(2);
    if (this.thuTrongTuan.thu3) thus.push(3);
    if (this.thuTrongTuan.thu4) thus.push(4);
    if (this.thuTrongTuan.thu5) thus.push(5);
    if (this.thuTrongTuan.thu6) thus.push(6);
    if (this.thuTrongTuan.thu7) thus.push(7);
    if (this.thuTrongTuan.cn)   thus.push(8);

    const payload = {
      pageNumber: page,
      pageSize: this.pageSize,
      tenLop: this.searchTerm,
      thus: thus,
      giaoVienCode: 'all',
      phongId: 0,
      chuongTrinhId: 0,
      trangThai: this.trangThai || 'all',
      thoiGianBatDau: this.timeStart || '',
      thoiGianKetThuc: this.timeEnd || '',
      ngayBatDau: this.dateStart || '0001-01-01',
      ngayKetThuc: this.dateEnd || '0001-01-01'
    };

    this.lophocService.getDanhSachLopHoc(payload).subscribe({
      next: (response) => {
        if (!response.isError && response.data) {
          const data = response.data;

          this.lophocs = data.lopHocs.map((item: any) => {
            const lichCoDinh = item.loaiLichHocs.find((l: any) => l.trangThai === 'Cố định')?.lichHocs || [];
            const firstLich = lichCoDinh[0];
          
            return {
              tenLop: item.tenLop,
              chuongTrinh: item.tenChuongTrinh,
              phong: firstLich?.tenPhong || '',
              giaoVien: item.tenGiaoVien,
              ngayBatDau: firstLich?.ngayBatDau,
              ngayKetThuc: firstLich?.ngayKetThuc,
              hocPhi: item.hocPhi,
              trangThai: item.loaiLichHocs.find((l: any) => l.lichHocs.length > 0)?.trangThai || 'Không xác định',
              
              lichHocChiTiet: lichCoDinh.map((lh: any) => ({
                thu: lh.thu,
                gioBatDau: lh.gioBatDau?.substring(0, 5),
                gioKetThuc: lh.gioKetThuc?.substring(0, 5),
                tenPhong: lh.tenPhong || ''
              }))
            };
          });
          

          this.currentPage = data.pageNumber;
          this.totalPages = Math.ceil(data.totalCount / this.pageSize);
          this.cdr.detectChanges();
        } else {
          this.lophocs = [];
          this.totalPages = 1;
        }
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách lớp học:', err);
        this.lophocs = [];
        this.totalPages = 1;
      }
    });
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadLopHocs(page);
  }

  onAddClass() {
    this.router.navigate(['/lophoc/add']);
  }

  onEditClass(index: number) {
    alert('Sửa lớp: ' + this.lophocs[index].tenLop);
  }
}

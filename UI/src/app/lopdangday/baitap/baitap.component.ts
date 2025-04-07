import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baitap',
  templateUrl: './baitap.component.html',
  styleUrls: ['./baitap.component.scss']
})
export class BaitapComponent implements OnInit {
  baiTaps: any[] = [];
  filteredBaiTaps: any[] = [];
  currentPage: number = 1;
  pageSize: number = 6;
  isAddModalOpen: boolean = false;
  trangThaiFilter: string = '';
  ngayFilter: string = '';
  newBaiTap: any = {
    tieuDe: '',
    noiDung: '',
    gioHetHan: '',
    files: [] as File[]
  };
  ngOnInit(): void {
    this.baiTaps = [
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2024-04-20', gio: '17h - 23h', trangThai: 'DangMo' },
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2025-04-20', gio: '17h - 23h', trangThai: 'DaDong' },
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2025-05-20', gio: '17h - 23h', trangThai: 'DangMo' },
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2025-04-22', gio: '17h - 23h', trangThai: 'DangMo' },
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2025-05-25', gio: '17h - 23h', trangThai: 'DangMo' },
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2025-04-26', gio: '17h - 23h', trangThai: 'DaDong' },
      { ten: 'Bài tập ôn tập hệ số góc', ngay: '2025-05-10', gio: '17h - 23h', trangThai: 'DangMo' },
    ];
    this.applyFilter();
  }

  applyFilter(): void {
    let data = this.baiTaps.slice();

    if (this.trangThaiFilter) {
      data = data.filter(bt => bt.trangThai === this.trangThaiFilter);
    }

    if (this.ngayFilter) {
      data = data.filter(bt => bt.ngay === this.ngayFilter);
    }

    this.filteredBaiTaps = data;
    this.currentPage = 1;
  }

  get paginatedBaiTaps() {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredBaiTaps.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredBaiTaps.length / this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }
  openAddModal(): void {
    this.isAddModalOpen = true;
    this.newBaiTap = {
      tieuDe: '',
      noiDung: '',
      gioHetHan: '',
      files: []
    };
  }
  
  closeAddModal(): void {
    this.isAddModalOpen = false;
  }
  
  handleFileInput(event: any): void {
    const selectedFiles = Array.from(event.target.files) as File[];
    this.newBaiTap.files = this.newBaiTap.files.concat(selectedFiles);

  }
  
  removeFile(index: number): void {
    this.newBaiTap.files.splice(index, 1);
  }
  
  submitNewBaiTap(): void {
    console.log('📩 Dữ liệu gửi đi:', this.newBaiTap);
    this.closeAddModal();
  }
  
}

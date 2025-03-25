import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ChinhSachService } from './shared/chinhsach.service';
import { ChinhSach } from './shared/chinhsach.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-chinhsach',
  templateUrl: './chinhsach.component.html',
  styleUrls: ['./chinhsach.component.scss']
})
export class ChinhsachComponent implements OnInit {
  danhSachChinhSach: ChinhSach[] = [];
  pagedChinhSach: ChinhSach[] = [];
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;

  constructor(
    private chinhSachService: ChinhSachService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadChinhSach();
  }

  loadChinhSach(): void {
    this.chinhSachService.getDanhSachChinhSach().subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.danhSachChinhSach = res.data;
          this.totalPages = Math.ceil(this.danhSachChinhSach.length / this.pageSize);
          this.setPagedData();
          this.cdr.detectChanges();
        } else {
          this.toastr.error('Không thể tải danh sách chính sách', 'Lỗi');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi gọi API chính sách:', err);
        this.toastr.error('Có lỗi khi gọi API!', 'Lỗi');
      }
    });
  }
  setPagedData() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedChinhSach = this.danhSachChinhSach.slice(startIndex, endIndex);
    this.cdr.detectChanges();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.setPagedData();
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.setPagedData();
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.setPagedData();
    }
  }
}

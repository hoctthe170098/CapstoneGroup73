import { Component, OnInit } from '@angular/core';
import { LopdanghocService } from '../shared/lopdanghoc.service';
import { ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute,Router } from '@angular/router';
@Component({
  selector: 'app-bai-tap',
  templateUrl: './bai-tap.component.html',
  styleUrls: ['./bai-tap.component.scss']
})
export class BaiTapComponent implements OnInit {
  baiTaps: any[] = [];
  filteredBaiTaps: any[] = [];
  trangThaiFilter: string = '';
  currentPage: number = 1;
  pageSize: number = 6;
  totalPages: number = 1;
  tenLop: string = '';

  constructor(
    private lopdanghocService: LopdanghocService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
      this.loadBaiTaps();
    });
  }

  loadBaiTaps() {
    const payload = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      tenLop: this.tenLop, 
      trangThai: this.trangThaiFilter || 'all'
    };
  
    this.lopdanghocService.getBaiTapsForStudent(payload).subscribe({
      next: (res) => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        if (!res.isError && res.data) {
          const items = res.data.items.flatMap((item: any) => item.baiTaps || []);
          this.baiTaps = items;
          this.totalPages = res.data.totalPages || 1;
        } else {
          this.baiTaps = [];
          this.totalPages = 1;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách bài tập:', err);
        this.baiTaps = [];
        this.totalPages = 1;
        this.cdr.detectChanges();
      }
    });
  }
  
  

  applyFilter(): void {
    this.currentPage = 1;
    this.loadBaiTaps();
  }

  get paginatedBaiTaps() {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredBaiTaps.slice(start, start + this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadBaiTaps();
    }
  }
}

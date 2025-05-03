import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { LopDangDay } from './shared/lopdangday.model';
import { LopdangdayService } from './shared/lopdangday.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: "app-lopdangday",
  templateUrl: "./lopdangday.component.html",
  styleUrls: ["./lopdangday.component.scss"],
})

export class LopdangdayComponent implements OnInit {
  classes: LopDangDay[] = [];
  pageNumber: number = 1;
  pageSize: number = 9;
  totalItems: number = 0;
  searchClass: string = '';
  startDate: string = '';
  endDate: string = '';

  today: Date = new Date();
  parseDate(dateString: string): Date {
    return new Date(dateString);
  }

  constructor( private spinner: NgxSpinnerService, private router: Router, private lopDangDayService: LopdangdayService, private cdr: ChangeDetectorRef ) { }
  

  ngOnInit(): void {
    this.loadLopdangday();
  }

  loadLopdangday() {
    // Hiển thị spinner trong khi load dữ liệu
    this.spinner.show();
    console.debug("today", this.today);
    // Gọi service và subscribe để nhận dữ liệu
    this.lopDangDayService.getDanhSachLopHoc(this.pageNumber, this.pageSize, this.searchClass, this.startDate, this.endDate).subscribe(
      (response) => {
        if (response.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        // Ẩn spinner sau khi dữ liệu được load
        this.spinner.hide();

        // Giả sử response có các thuộc tính 'classes' và 'totalItems'
        this.classes = response.data.items.map((lop: any) =>{
          const ngayBatDau = this.parseDate(lop.ngayBatDau);
          const ngayKetThuc = this.parseDate(lop.ngayKetThuc);
          let status = '';
        
          if (this.today < ngayBatDau) {
            status = 'Chưa bắt đầu';
          } else if (this.today >= ngayBatDau && this.today <= ngayKetThuc) {
            status = 'Đang hoạt động';
          } else {
            status = 'Kết thúc';
          }
        
          return {
            ...lop,
            status
          };
        });
        
        this.totalItems = response.data.totalCount;
        this.cdr.detectChanges(); // Cập nhật view nếu cần thiết
      },
      (error) => {
        // Ẩn spinner nếu có lỗi
        this.spinner.hide();
        console.error('Error fetching classes', error);
      }
    );
  }

  // Hàm gọi lại API khi tìm kiếm
  searchClasses(): void {
    this.pageNumber = 1; // reset trang về 1 khi tìm kiếm
    this.loadLopdangday();
  }

  goToDetail(tenLop: string) {
    this.router.navigate(['/lopdangday', 'chi-tiet', tenLop]);
  }

  // Phân trang
  changePage(page: number) { this.pageNumber = page; this.loadLopdangday(); }
  get totalPages() { return Math.ceil(this.totalItems / this.pageSize); }
  getPageNumbers(): number[] {
      const pages: number[] = [];
      for (let i = 1; i <= this.totalPages; i++) {
          pages.push(i);
      }
      return pages;
  }
}

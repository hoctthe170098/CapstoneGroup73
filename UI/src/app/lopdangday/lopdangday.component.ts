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

  constructor( private spinner: NgxSpinnerService, private router: Router, private lopDangDayService: LopdangdayService, private cdr: ChangeDetectorRef ) { }
  

  ngOnInit(): void {
    this.loadLopdangday();
  }

  loadLopdangday() {
    // Hiển thị spinner trong khi load dữ liệu
    this.spinner.show();

    // Gọi service và subscribe để nhận dữ liệu
    this.lopDangDayService.getDanhSachLopHoc(this.pageNumber, this.pageSize, this.searchClass, this.startDate, this.endDate).subscribe(
      (response) => {
        // Ẩn spinner sau khi dữ liệu được load
        this.spinner.hide();

        console.debug('Response từ API:', response);
        // Giả sử response có các thuộc tính 'classes' và 'totalItems'
        this.classes = response.data.items;
        this.totalItems = response.data.totalCount;
        this.cdr.detectChanges(); // Cập nhật view nếu cần thiết
        console.debug('Danh sách lớp học:', this.classes);
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

  goToDetail(lopId: string) {
    this.router.navigate(['/lopdangday', 'chi-tiet', lopId]);
  }
}

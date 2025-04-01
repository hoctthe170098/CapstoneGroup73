import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { LopDangDay } from './shared/lopdangday.model';
import { LopdangdayService } from './shared/lopdangday.service';
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

  constructor( private spinner: NgxSpinnerService, private router: Router, private lopDangDayService: LopdangdayService ) { }
  

  ngOnInit(): void {
    console.debug('LopdangdayComponent ngOnInit has been called!'); 
    // Hiển thị spinner trong khi load dữ liệu
    this.spinner.show();
  
    // Tạo payload dựa trên các giá trị của component
    const payload = {
      search: this.searchClass,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      startDate: this.startDate,
      endDate: this.endDate
    };
  
    // Gọi service và subscribe để nhận dữ liệu
    this.lopDangDayService.getDanhSachLopHoc(payload).subscribe(
      (response) => {
        // Ẩn spinner sau khi dữ liệu được load
        this.spinner.hide();
  
        // Giả sử response có các thuộc tính 'classes' và 'totalItems'
        this.classes = response.classes;
        this.totalItems = response.totalItems;
        console.debug('Danh sách lớp học:', this.classes);
      },
      (error) => {
        // Ẩn spinner nếu có lỗi
        this.spinner.hide();
        console.error('Error fetching classes', error);
      }
    );
  }

  goToDetail(lopId: string) {
    this.router.navigate([`/lopdangday/chi-tiet/${lopId}`]);
  }
}

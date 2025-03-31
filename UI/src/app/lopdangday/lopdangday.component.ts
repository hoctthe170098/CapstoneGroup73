import { Component, OnInit } from "@angular/core";
import { LopdangdayService } from "./shared/lopdangday.service";
import { LopDangDay } from "./shared/lopdangday.model";

@Component({
  selector: "app-lopdangday",
  templateUrl: "./lopdangday.component.html",
  styleUrls: ["./lopdangday.component.scss"],
})
export class LopdangdayComponent implements OnInit {
  classes: LopDangDay[] = [];

  // Các thông số phân trang và tìm kiếm
  pageNumber: number = 1;
  pageSize: number = 9;
  totalPages: number = 1;
  searchClass: string = '';
  startDate: string = '';
  endDate: string = '';

  constructor(private lopdangdayService: LopdangdayService) {}

  ngOnInit(): void {
    this.loadClasses();
  }

  // Hàm gọi service để lấy danh sách lớp
  loadClasses(): void {
    const payload = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchClass: this.searchClass,
      startDate: this.startDate,
      endDate: this.endDate,
    };

    this.lopdangdayService.getDanhSachLopHoc(payload).subscribe({
      next: (response) => {
        if (response && !response.isError) {
          this.classes = response.data;
        } else {
          console.error("Lỗi khi lấy danh sách lớp:", response);
        }
      },
      error: (err) => {
        console.error("Có lỗi xảy ra khi gọi API:", err);
      },
    });
  }

    // Hàm chuyển trang: quay về trang trước đó
    prevPage(): void {
      if (this.pageNumber > 1) {
        this.pageNumber--;
        this.loadClasses();
      }
    }
  
    // Hàm chuyển trang: chuyển sang trang kế tiếp
    nextPage(): void {
      if (this.pageNumber < this.totalPages) {
        this.pageNumber++;
        this.loadClasses();
      }
    }
  
    // Hàm chuyển sang trang được chọn
    goToPage(page: number): void {
      if (page !== this.pageNumber) {
        this.pageNumber = page;
        this.loadClasses();
      }
    }
  
    // Hàm trả về mảng số trang dựa trên tổng số trang
    getPages(): number[] {
      const pages: number[] = [];
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
      return pages;
    }
}

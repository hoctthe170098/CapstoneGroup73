import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { LopdanghocService } from "./shared/lopdanghoc.service";
import { LopDangHoc } from "./shared/lopdanghoc.model";

@Component({
  selector: "app-lopdanghoc",
  templateUrl: "./lopdanghoc.component.html",
  styleUrls: ["./lopdanghoc.component.scss"],
})
export class LopdanghocComponent implements OnInit {
  classes: LopDangHoc[] = [];
  pageNumber: number = 1;
  pageSize: number = 9;
  totalItems: number = 0;
  searchClass: string = '';

  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    private lopDangHocService: LopdanghocService,
    private cdr: ChangeDetectorRef
  ) {}

  today: Date = new Date();
  parseDate(dateString: string): Date {
    return new Date(dateString);
  }

  ngOnInit(): void {
    this.loadLopdanghoc();
  }

  loadLopdanghoc() {
    this.spinner.show();
    // Gọi service và subscribe để nhận dữ liệu
    this.lopDangHocService
      .getDanhSachLopHoc(this.pageNumber, this.pageSize, this.searchClass)
      .subscribe(
        (response) => {
          // Ẩn spinner sau khi dữ liệu được load
          this.spinner.hide();

          // Giả sử response có các thuộc tính 'classes' và 'totalItems'
          this.classes = response.data.items.map((lop: any) => {
            const ngayBatDau = this.parseDate(lop.ngayBatDau);
            const ngayKetThuc = this.parseDate(lop.ngayKetThuc);
            let status = "";

            if (this.today < ngayBatDau) {
              status = "Chưa bắt đầu";
            } else if (this.today >= ngayBatDau && this.today <= ngayKetThuc) {
              status = "Đang hoạt động";
            } else {
              status = "Kết thúc";
            }

            return {
              ...lop,
              status,
            };
          });

          this.totalItems = response.data.totalCount;
          this.cdr.detectChanges(); // Cập nhật view nếu cần thiết
        },
        (error) => {
          // Ẩn spinner nếu có lỗi
          this.spinner.hide();
          console.error("Error fetching classes", error);
        }
      );
  }

  searchClasses(): void {
    this.pageNumber = 1; // reset trang về 1 khi tìm kiếm
    this.loadLopdanghoc();
  }

  goToDetail(tenLop: string) {
    this.router.navigate(['/lopdanghoc', 'chi-tiet', tenLop, 'bai-tap']);
  }

  // Phân trang
  changePage(page: number) { this.pageNumber = page; this.loadLopdanghoc(); }
  get totalPages() { return Math.ceil(this.totalItems / this.pageSize); }
  getPageNumbers(): number[] {
      const pages: number[] = [];
      for (let i = 1; i <= this.totalPages; i++) {
          pages.push(i);
      }
      return pages;
  }
}

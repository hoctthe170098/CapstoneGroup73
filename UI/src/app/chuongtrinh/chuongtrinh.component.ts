import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ChuongtrinhService } from './shared/chuongtrinh.service';
import { Router } from '@angular/router';
import { saveAs } from 'file-saver';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-chuongtrinh',
  templateUrl: './chuongtrinh.component.html',
  styleUrls: ['./chuongtrinh.component.scss']
})
export class ChuongtrinhComponent implements OnInit {
  programs: any[] = [];
  currentPage = 1;
  totalPages = 1;
  searchQuery: string = ''; // ✅ Biến lưu từ khóa tìm kiếm

  constructor(
    private chuongtrinhService: ChuongtrinhService,
    private router: Router,
    private cdr: ChangeDetectorRef, private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.getPrograms(this.currentPage, this.searchQuery);
  }

  /**
   * Gọi API lấy danh sách chương trình có phân trang & tìm kiếm theo tiêu đề
   */
  getPrograms(page: number, search: string): void {
    this.chuongtrinhService.getPrograms(page, search).subscribe(response => {
      console.log("📌 Dữ liệu từ API:", response);
      if (!response.isError && response.code === 200) {
        // ✅ Chỉ lọc theo tiêu đề (tieuDe)
        this.programs = response.data.items
          .filter(program => program.tieuDe.toLowerCase().includes(search.toLowerCase()))
          .map(program => ({ ...program, expanded: false }));

        this.currentPage = response.data.pageNumber;
        this.totalPages = response.data.totalPages;
        this.cdr.detectChanges();
      } else {
        console.warn("⚠️ Lỗi khi lấy danh sách chương trình:", response.message);
      }
    });
  }

  /**
   * Toggle hiển thị nội dung bài học của chương trình
   */
  toggleContent(id: number, event: Event) {
    event.preventDefault();
    const program = this.programs.find(p => p.id === id);
    if (program) {
      program.expanded = !program.expanded;
      this.cdr.detectChanges();
    }
  }

  /**
   * Tìm kiếm chương trình học
   */
  onSearch() {
    this.currentPage = 1;
    this.getPrograms(this.currentPage, this.searchQuery);
  }

  /**
   * Chuyển trang (Trang trước / Trang sau)
   */
  changePage(next: boolean) {
    if (next && this.currentPage < this.totalPages) {
      this.currentPage++;
    } else if (!next && this.currentPage > 1) {
      this.currentPage--;
    }
    this.getPrograms(this.currentPage, this.searchQuery);
  }
  onDeleteProgram(id: number, event: Event): void {
    event.preventDefault(); // ✅ Ngăn chặn reload

    if (confirm(`Bạn có chắc chắn muốn xóa chương trình ID ${id} không?`)) {
      this.chuongtrinhService.deleteProgram(id).subscribe({
        next: (response) => {
          console.log(`✅ Xóa thành công chương trình ID ${id}:`, response);
          this.programs = this.programs.filter(program => program.id !== id);
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error(`❌ Lỗi khi xóa chương trình ID ${id}:`, error);

          if (error.status === 401) {
            alert("⚠️ Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.");
          }
        }
      });
    }
  }

  downloadFile(fileUrl: string, fileName: string): void {
    this.chuongtrinhService.downloadFile(fileUrl).subscribe(
      (res: any) => {
        if (res instanceof Blob) {
          // Trường hợp thành công (nhận Blob)
          saveAs(res, fileName);
        } else if (res && res.isError) {
          // Trường hợp lỗi (nhận res JSON)
          this.toastr.error(res.message);
        }
      },
      (error: any) => {
        // Trường hợp lỗi HTTP
        console.error('Lỗi khi tải file:', error);
        this.toastr.error('Đã có lỗi xảy ra.');
      }
    );
  }
  // downloadFile(fileUrl: string,fileName:string): void {
  //   this.chuongtrinhService.downloadFile(fileUrl).subscribe((blob: Blob) => {
  //     saveAs(blob, fileName);
  //   });
  // }
}

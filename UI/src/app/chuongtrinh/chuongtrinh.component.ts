import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ChuongtrinhService } from './shared/chuongtrinh.service';
import { Router } from '@angular/router';

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
    private cdr: ChangeDetectorRef
  ) {}

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



  downloadFile(event: Event, fileUrl: string, fileName: string): void {
    event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>

    console.log("📌 Đang tải file:", fileUrl, fileName); // ✅ Kiểm tra dữ liệu

    if (!fileUrl) {
        console.warn("⚠️ Không có URL tải file!");
        return;
    }

    fetch(fileUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error("Lỗi tải file: " + response.statusText);
            }
            return response.blob();
        })
        .then(blob => {
            const link = document.createElement("a");
            const url = window.URL.createObjectURL(blob);
            link.href = url;
            link.download = fileName || "tai-lieu.pdf"; // Đặt tên mặc định nếu không có
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url); // Giải phóng bộ nhớ
        })
        .catch(error => console.error("Lỗi tải file:", error));
        
}




}

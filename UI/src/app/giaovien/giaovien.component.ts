import { Component, OnInit, ChangeDetectorRef  } from '@angular/core';
import { Giaovien } from './shared/giaovien.model';
import { GiaovienService } from './shared/giaovien.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-giaovien',
  templateUrl: './giaovien.component.html',
  styleUrls: ['./giaovien.component.scss']
})
export class GiaovienComponent implements OnInit {
  trangThai: string = ''; 
  searchTerm: string = ''; 
  students: any[] = [];
  currentPage: number = 1;
  totalPages: number = 1;
  pageSize: number = 8;
  
  constructor(private giaovienService: GiaovienService, private router: Router, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadDanhSachGiaoVien();
  }

  /** Load danh sách giáo viên từ API */
  loadDanhSachGiaoVien() {
    let isActiveFilter: boolean | null;
    
    if (this.trangThai === 'Hoạt động') {
      isActiveFilter = true;
    } else if (this.trangThai === 'Tạm ngừng') {
      isActiveFilter = false;
    } else {
      isActiveFilter = null;
    }

    console.log(`🔎 Filter trạng thái: ${this.trangThai}, isActive =`, isActiveFilter);

    this.giaovienService.getDanhSachGiaoVien(this.currentPage, this.pageSize, this.searchTerm, '', isActiveFilter)
      .subscribe(response => {
        if (!response.isError && response.data) {
          this.students = response.data.map((gv: any) => ({
            code: gv.code || '',
            ten: gv.ten || '',
            gioiTinh: gv.gioiTinh || '',
            diaChi: gv.diaChi || '',
            truongDangDay: gv.truongDangDay || '',
            ngaySinh: gv.ngaySinh ? new Date(gv.ngaySinh) : null,
            email: gv.email || '',
            soDienThoai: gv.soDienThoai || '',
            isActive: gv.isActive !== undefined ? gv.isActive : false,
            tenCoSo: gv.tenCoSo || '',
            lopHocs: gv.tenLops ? gv.tenLops : [],
            showDetails: false
          }));

          this.totalPages = Math.ceil(response.data.length / this.pageSize);
        } else {
          console.error("Lỗi khi tải danh sách giáo viên", response.message);
        }
        this.cdr.detectChanges(); // Kích hoạt Change Detection để UI cập nhật ngay lập tức
      }, error => {
        console.error("Lỗi kết nối API", error);
      });
  }
  

  /** Chuyển trang */
  goToPage(page: number) {
    this.currentPage = page;
    this.loadDanhSachGiaoVien();
  }

  /** Tìm kiếm giáo viên */
  searchGiaoVien() {
    this.currentPage = 1;
    this.loadDanhSachGiaoVien();
  }

  /** Mở rộng chi tiết giáo viên */
  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  /** Thêm giáo viên */
  isModalOpen: boolean = false;
  newStudent: any = {
    code: '',
    ten: '',
    gioiTinh: 'Nam',
    ngaySinh: '',
    email: '',
    soDienThoai: '',
    truongDangDay: '',
    diaChi: ''
  };

  openAddStudentModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  submitNewStudent() {
    if (!this.newStudent.ten || !this.newStudent.email || !this.newStudent.soDienThoai) {
      alert("Vui lòng điền đầy đủ thông tin!");
      return;
    }

    this.giaovienService.createGiaoVien(this.newStudent).subscribe(response => {
      if (!response.isError) {
        alert(response.message);
        this.loadDanhSachGiaoVien();
        this.closeModal();
      } else {
        alert("Lỗi khi thêm giáo viên: " + response.message);
      }
    }, error => {
      console.error("Lỗi kết nối API", error);
    });
  }

  /** Chỉnh sửa giáo viên */
  isEditModalOpen: boolean = false;
  editStudent: any = {
    code: '',
    ten: '',
    gioiTinh: 'Nam',
    ngaySinh: '',
    email: '',
    soDienThoai: '',
    truongDangDay: '',
    diaChi: ''
  };

  onEditStudentClick(index: number) {
    this.editStudent = { ...this.students[index] };
    this.isEditModalOpen = true;
  }

  closeEditModal() {
    this.isEditModalOpen = false;
  }

  submitEditStudent() {
    this.giaovienService.updateGiaoVien(this.editStudent).subscribe(response => {
      if (!response.isError) {
        alert(response.message);
        this.loadDanhSachGiaoVien();
        this.closeEditModal();
      } else {
        alert("Lỗi khi cập nhật giáo viên: " + response.message);
      }
    }, error => {
      console.error("Lỗi kết nối API", error);
    });
  }
  filterByStatus() {
    this.currentPage = 1;
    this.loadDanhSachGiaoVien();
  }
  /** Hàm format ngày => yyyy-MM-dd */
  formatDate(date: Date) {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
    onChooseFileclick() {
      this.router.navigate(['/giaovien/import-giao-vien']);
    }
  
    onExportFile() {
      alert('Xuất Tệp');
    }
  

}

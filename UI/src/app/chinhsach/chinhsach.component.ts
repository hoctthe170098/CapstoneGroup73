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
  currentPage: number = 1;
  totalPages: number = 1;
  pageSize: number = 5;

  ten: string = '';
  mota: string = '';
  phanTramGiam: number | null = null;

  isSubmitted: boolean = false;
  isEditModalOpen: boolean = false;
  editId: number | null = null;
  editTen: string = '';
  editMoTa: string = '';
  editPhanTramGiam: number | null = null;
  isSubmittedEdit: boolean = false;
  constructor(
    private chinhSachService: ChinhSachService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadChinhSachs();
  }
 
  closeEditModal(): void {
    this.isEditModalOpen = false;
  }
  
  
  loadChinhSachs(): void {
    const payload = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize
    };

    this.chinhSachService.getChinhSachs(payload).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.danhSachChinhSach = res.data.items;
          this.totalPages = res.data.totalPages;
          this.currentPage = res.data.pageNumber;
          this.cdr.detectChanges();
        } else {
          this.danhSachChinhSach = [];
          this.totalPages = 1;
        }
      },
      error: (err) => {
        console.error('Lỗi khi gọi API chính sách:', err);
        this.danhSachChinhSach = [];
        this.totalPages = 1;
      }
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadChinhSachs();
    }
  }

  addChinhSach(): void {
    this.isSubmitted = true;

    const tenTrimmed = this.ten.trim();

    if (!tenTrimmed || this.phanTramGiam === null || this.phanTramGiam < 0 || this.phanTramGiam > 100) {
      return;
    }

    const body = {
      ten: tenTrimmed,
      mota: this.mota?.trim() || '',
      phanTramGiam: this.phanTramGiam / 100
    };

    this.chinhSachService.createChinhSach(body).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.toastr.success(res.message || 'Thêm chính sách thành công!');
          this.resetForm();
          this.loadChinhSachs();
        } else {
          this.toastr.error(res.message || 'Không thể thêm chính sách!');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi thêm chính sách:', err);
        this.toastr.error('Có lỗi xảy ra khi thêm chính sách!');
      }
    });
  }

  resetForm(): void {
    this.ten = '';
    this.mota = '';
    this.phanTramGiam = null;
    this.isSubmitted = false;
  }
  xoaChinhSach(id: number): void {
    const confirmDelete = confirm('Bạn có chắc muốn xóa chính sách này?');
  
    if (!confirmDelete) return;
  
    this.chinhSachService.deleteChinhSach(id).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Xóa chính sách thành công!');
          this.loadChinhSachs(); 
        } else {
          this.toastr.error(res.message || 'Không thể xóa chính sách!');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi xóa chính sách:', err);
        this.toastr.error('Có lỗi xảy ra khi xóa chính sách!');
      }
    });
  }
  // Mở modal sửa
  openEditModal(cs: ChinhSach): void {
    this.editId = cs.id;
    this.editTen = cs.ten;
    this.editMoTa = cs.mota;
    this.editPhanTramGiam = cs.phanTramGiam * 100;
    this.isSubmittedEdit = false;
    this.isEditModalOpen = true;
  }

  // Gửi cập nhật
  applyEdit(): void {
    this.isSubmittedEdit = true;
    const ten = this.editTen.trim();
    const giam = this.editPhanTramGiam;

    if (!this.editId || !ten || giam === null || giam < 0 || giam > 100) return;

    const body = {
      id: this.editId,
      ten,
      mota: this.editMoTa?.trim() || '',
      phanTramGiam: giam / 100
    };

    this.chinhSachService.updateChinhSach(body).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Cập nhật thành công!');
          this.isEditModalOpen = false;
          this.loadChinhSachs();
        } else {
          this.toastr.error(res.message || 'Không thể cập nhật!');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi cập nhật:', err);
        this.toastr.error('Đã xảy ra lỗi khi cập nhật chính sách!');
      }
    });
  }

  
}

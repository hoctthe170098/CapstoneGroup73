import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ChinhSachService } from './shared/chinhsach.service';
import { ChinhSach } from './shared/chinhsach.model';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
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
    private cdr: ChangeDetectorRef,
    private router:Router
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
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
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
    const motaTrimmed = this.mota?.trim() || '';
  
    const isInvalid =
      !tenTrimmed ||
      tenTrimmed.length > 30 ||
      motaTrimmed.length > 200 ||
      this.phanTramGiam === null ||
      isNaN(this.phanTramGiam) ||
      this.phanTramGiam < 0 ||
      this.phanTramGiam > 10;
  
    if (isInvalid) {
      return; 
    }
  
    const body = {
      ten: tenTrimmed,
      mota: motaTrimmed,
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
  openEditModal(cs: ChinhSach): void {
    this.editId = cs.id;
    this.editTen = cs.ten;
    this.editMoTa = cs.mota;
    this.editPhanTramGiam = cs.phanTramGiam * 100;
    this.isSubmittedEdit = false;
    this.isEditModalOpen = true;
  }

  applyEdit(): void {
    this.isSubmittedEdit = true;
  
    const ten = this.editTen.trim();
    const mota = this.editMoTa?.trim() || '';
    const giam = this.editPhanTramGiam;
  
    const isInvalid =
      !this.editId ||
      !ten ||
      ten.length > 30 ||
      mota.length > 200 ||
      giam === null ||
      isNaN(giam) ||
      giam < 0 ||
      giam > 10;
  
    if (isInvalid) {
      return; 
    }
  
    const confirmResult = window.confirm('Bạn có chắc muốn cập nhật chính sách này?');
    if (!confirmResult) return;
  
    const body = {
     
        id: this.editId,
        ten,
        mota,
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
      error: () => {
        this.toastr.error('Đã xảy ra lỗi khi cập nhật chính sách!');
      }
    });
  }
  
  

  
}

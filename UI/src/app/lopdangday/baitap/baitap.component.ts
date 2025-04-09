import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LopdangdayService } from '../shared/lopdangday.service';
import { ToastrService } from 'ngx-toastr';
import { Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router'; 
@Component({
  selector: 'app-baitap',
  templateUrl: './baitap.component.html',
  styleUrls: ['./baitap.component.scss']
})
export class BaitapComponent implements OnInit {
  @Input() tenLop: string = '';
  baiTaps: any[] = [];
  filteredBaiTaps: any[] = [];
  currentPage: number = 1;
  pageSize: number = 6;
  totalPages: number = 1;

  isAddModalOpen: boolean = false;

  trangThaiFilter: string = '';
  
  newBaiTap: any = {
    tieuDe: '',
    noiDung: '',
    gioHetHan: '',
    files: [] as File[]
  };
  formErrors = {
    thoiGianKetThuc: '',
    tieuDe: '',
    noiDung: '',
    file: ''
  };
  constructor(
    private lopdangdayService: LopdangdayService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
  
    this.route.parent?.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
     
      this.loadBaiTaps(); 
    });
  }

  loadBaiTaps(): void {
    const payload = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      tenLop: this.tenLop,
      trangThai: this.trangThaiFilter || 'all'
    };

    this.lopdangdayService.getBaiTapsForTeacher(payload).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          const allBaiTaps = res.data.items.flatMap((item: any) => item.baiTaps || []);
          this.baiTaps = allBaiTaps;
          this.filteredBaiTaps = allBaiTaps;
          this.totalPages = res.data.totalPages || 1;
        } else {
          this.baiTaps = [];
          this.filteredBaiTaps = [];
          this.totalPages = 1;
        }

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi khi lấy danh sách bài tập:', err);
        this.baiTaps = [];
        this.filteredBaiTaps = [];
        this.totalPages = 1;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilter(): void {
    this.currentPage = 1;
    this.loadBaiTaps();
  }

  get paginatedBaiTaps() {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredBaiTaps.slice(start, start + this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadBaiTaps();
    }
  }

  openAddModal(): void {
    this.isAddModalOpen = true;
    this.newBaiTap = {
      tieuDe: '',
      noiDung: '',
      gioHetHan: '',
      files: [],
      tenLop: 'Lớp toán 6'
    };
  }

  closeAddModal(): void {
    this.isAddModalOpen = false;
  }

  handleFileInput(event: any): void {
    const selectedFiles = Array.from(event.target.files) as File[];
    this.newBaiTap.files = this.newBaiTap.files.concat(selectedFiles);

    if (selectedFiles.length > 0) {
      this.formErrors.file = '';
    }
  }
  

  removeFile(index: number): void {
    this.newBaiTap.files.splice(index, 1);
  }

  submitNewBaiTap(): void {
    this.formErrors = { thoiGianKetThuc: '', tieuDe: '', noiDung: '', file: '' };

 
    if (!this.newBaiTap.thoiGianKetThuc) {
      this.formErrors.thoiGianKetThuc = 'Vui lòng chọn thời gian hết hạn';
    }
  
    if (!this.newBaiTap.tieuDe?.trim()) {
      this.formErrors.tieuDe = 'Tiêu đề không được để trống';
    }
  
    if (!this.newBaiTap.noiDung?.trim()) {
      this.formErrors.noiDung = 'Nội dung không được để trống';
    }
  
    if (!this.newBaiTap.files || this.newBaiTap.files.length === 0) {
      this.formErrors.file = 'Vui lòng chọn ít nhất 1 tệp';
    }
  
    const hasError = Object.values(this.formErrors).some(msg => msg !== '');
    if (hasError) {
      return;
    }

    const formData = new FormData();
    formData.append('CreateBaiTapDto.tenLop', this.tenLop);
    formData.append('CreateBaiTapDto.tieuDe', this.newBaiTap.tieuDe);
    formData.append('CreateBaiTapDto.noiDung', this.newBaiTap.noiDung);
  
    const validTime = this.newBaiTap.thoiGianKetThuc;
    formData.append('CreateBaiTapDto.thoiGianKetThuc', validTime);
  
    if (this.newBaiTap.files.length > 0) {
      formData.append('CreateBaiTapDto.taiLieu', this.newBaiTap.files[0]);
    }


   
  
    this.lopdangdayService.createBaiTap(formData).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Tạo bài tập thành công!');
          this.closeAddModal();
          this.loadBaiTaps();
        } else {
          this.toastr.error(res.message || 'Tạo bài tập thất bại!');
        }
      },
      error: (err) => {
        this.toastr.error(err?.error?.message || 'Đã xảy ra lỗi!');
      }
    });
  }
  

  confirmDelete(bt: any): void {
    const confirmed = confirm(`Bạn có chắc chắn muốn xoá bài tập "${bt.tieuDe}"?`);
    if (confirmed) {
      this.deleteBaiTap(bt.id);
    }
  }
  
  deleteBaiTap(id: string): void {
    this.lopdangdayService.deleteBaiTap(id).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Xoá bài tập thành công!');
          this.loadBaiTaps();
        } else {
          this.toastr.error(res.message || 'Xoá bài tập thất bại!');
        }
      },
      error: (err) => {
        console.error('Lỗi khi xoá bài tập:', err);
        this.toastr.error('Đã xảy ra lỗi khi xoá bài tập!');
      }
    });
  }
  
}

import { Component, OnInit,ChangeDetectorRef  } from '@angular/core';
import { TestlistService } from './shared/baikiemtra.service';
import { BaiKiemTraDto } from './shared/baikiemtra.model';
import { saveAs } from 'file-saver';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-testlist',
  templateUrl: './baikiemtra.component.html',
  styleUrls: ['./baikiemtra.component.scss']
})
export class TestListComponent implements OnInit {
  statusList = ['Đã kiểm tra', 'Chưa kiểm tra'];
 classList: string[] = [];
 selectedClassSchedule: any[] = [];
  selectedStatus = 'all';
  selectedClass = 'all';
  searchText = '';
  classDropdownOpen: boolean = false;
  classSearchTerm: string = '';
  filteredClassListForFilter: string[] = [];


  testList: any[] = [];

  // Phân trang
  paginatedList = [];
  totalItems = 0;

  currentPage = 1;
  itemsPerPage = 2;
  lopDropdownOpen = false;
  lopSearchTerm = '';
  filteredClassList: string[] = [];
  showCreateForm = false;

  newTest: BaiKiemTraDto = {
    tenBaiKiemTra: '',
    ngayKiemTra: '',
    tenLop: '',
    taiLieu: null as any 
  };
  showEditForm = false;
  editTest: any = {
    name: '',
    testDate: '',
    tenLop:'',
    taiLieu: {
      name: '',
      url: ''
    }
  };
  constructor(private testlistService: TestlistService,private cdr: ChangeDetectorRef, private toastr: ToastrService) {}
  ngOnInit() {
    this.loadTests();
    this.searchClassByName(''); // load tất cả lớp
    
  }
  toggleClassDropdown() {
    this.classDropdownOpen = !this.classDropdownOpen;
    this.classSearchTerm = '';
    this.filteredClassListForFilter = this.classList.slice(); // reset danh sách đầy đủ
  }

  // Hàm lọc danh sách lớp dựa trên từ khóa nhập
  onClassSearchTermChange() {
    const lower = this.classSearchTerm.toLowerCase();
    this.filteredClassListForFilter = this.classList.filter(cls =>
      cls.toLowerCase().includes(lower)
    );
  }
  onSearchTextChanged() {
    this.currentPage = 1;
    this.loadTests();
  }

  // Hàm chọn lớp và load lại danh sách bài kiểm tra
  selectClassFilter(selected: string) {
    this.selectedClass = selected;
    this.classDropdownOpen = false;
    this.loadTests();
  }
  loadTests() {
    const trangThaiToSend = this.selectedStatus || 'all';
    const tenLop = this.selectedClass || 'all';  
    const tenBaiKiemTra = this.searchText?.trim() || '';
    ;
  
    this.testlistService.getTests(this.currentPage, this.itemsPerPage, trangThaiToSend, tenLop, tenBaiKiemTra)
      .subscribe({
        next: res => {
          
          if (!res || !res.data) {
            this.paginatedList = [];
            this.totalItems = 0;
            // Bắt buộc cập nhật giao diện ngay
            this.cdr.detectChanges();
            return;
          }
  
          const responseData = res.data;
          this.totalItems = responseData.totalCount || 0;
          this.paginatedList = Array.isArray(responseData.data) ? responseData.data : [];
  
          // Gọi detectChanges() sau khi cập nhật dữ liệu để trigger change detection
          this.cdr.detectChanges();
        },
        error: err => {
          this.paginatedList = [];
          this.totalItems = 0;
          // Gọi detectChanges() ở trường hợp lỗi
          this.cdr.detectChanges();
        }
      });
  }
  
  downloadFile(fileUrl: string, fileName: string): void {
    if (!fileUrl) {
      this.toastr.warning('Không có tài liệu để tải!');
      return;
    }
  
    
  
    this.testlistService.downloadFile(fileUrl).subscribe(
      (res: Blob) => {
        saveAs(res, fileName);
      },
      (error: any) => {
        console.error('Lỗi tải file:', error);
        this.toastr.error('Tải tài liệu thất bại!');
      }
    );
  }
  
  // Tách đường dẫn tương đối từ URL
  
  

  updatePaginatedList() {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    this.paginatedList = this.testList.slice(start, end);
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages.length) return;
    this.currentPage = page;
    this.loadTests();
  }

  get totalPages(): number[] {
    const total = Math.ceil(this.totalItems / this.itemsPerPage);
    return total > 0 ? Array.from({ length: total }, (_, i) => i + 1) : [];
  }

  getFileIcon(fileName: string): string {
    if (fileName.endsWith('.pdf')) return 'fa fa-file-pdf';
    if (fileName.endsWith('.zip')) return 'fa fa-file-archive';
    if (fileName.endsWith('.doc') || fileName.endsWith('.docx') || fileName.endsWith('.docs')) return 'fa fa-file-word';
    return 'fa fa-file';
  }

  // ===============================
  // TẠO BÀI KIỂM TRA
  // ===============================
  onFileSelected(event: any, mode: 'create' | 'edit' = 'create') {
    const file = event.target.files[0];
    if (file) {
      if (mode === 'create') {
        this.newTest.taiLieu = file;
      } else {
        this.editTest.taiLieu = file;
        this.editTest.document = null; // Xoá file cũ nếu chọn file mới
      }
    }
  }
  
  allClassData: any[] = [];

searchClassByName(name: string) {
  const tenLop = name?.trim() || '';

  this.testlistService.getLopByName(tenLop).subscribe({
    next: (res: any) => {
      if (res?.data) {
        this.allClassData = res.data;
        this.classList = res.data.map((item: any) => item.tenLop);
      } else {
        this.classList = [];
        this.allClassData = [];
      }
    },
    error: err => {
     
      this.classList = [];
      this.allClassData = [];
    }
  });
}
onClassSelected(tenLop: string) {
  const selected = this.allClassData.find(item => item.tenLop === tenLop);
  this.selectedClassSchedule = selected?.lichHocs || [];
}
toggleLopDropdown() {
  this.lopDropdownOpen = !this.lopDropdownOpen;
  this.lopSearchTerm = '';
  this.filteredClassList = this.classList.slice(); // reset về đầy đủ
}

onLopSearchTermChange() {
  const lower = this.lopSearchTerm.toLowerCase();
  this.filteredClassList = this.classList.filter(cls =>
    cls.toLowerCase().includes(lower)
  );
}

selectLop(selected: string) {
  this.newTest.tenLop = selected;
  this.lopDropdownOpen = false;
  this.onClassSelected(selected); // nếu bạn đang dùng để lấy lịch
}

addTest() {
  if (!this.newTest.tenBaiKiemTra || !this.newTest.ngayKiemTra || !this.newTest.tenLop || !this.newTest.taiLieu) {
    this.toastr.warning('Vui lòng điền đầy đủ thông tin!');
    return;
  }

  const formData = new FormData();
  formData.append('BaiKiemTraDto.TenBaiKiemTra', this.newTest.tenBaiKiemTra);
  formData.append('BaiKiemTraDto.NgayKiemTra', this.newTest.ngayKiemTra);
  formData.append('BaiKiemTraDto.TenLop', this.newTest.tenLop);
  formData.append('BaiKiemTraDto.TaiLieu', this.newTest.taiLieu);

  this.testlistService.createTest(formData).subscribe({
    next: (res) => {
      if (res?.isError) {
        this.toastr.error(res.message || 'Tạo bài kiểm tra thất bại.');
        return;
      }

      this.toastr.success('Tạo bài kiểm tra thành công!');
      this.newTest = {
        tenBaiKiemTra: '',
        ngayKiemTra: '',
        tenLop: '',
        taiLieu: null as any
      };
      this.selectedClassSchedule = [];
      this.showCreateForm = false;
      this.loadTests();
    },
    error: (error) => {
      const res = error?.error;
      if (res?.isError) {
        this.toastr.error(res.message || res.errors?.[0] || 'Tạo bài kiểm tra thất bại!');
      } else {
        this.toastr.error('Đã xảy ra lỗi không xác định.');
      }
    }
  });
}

  openModal() {
    this.showCreateForm = true;
    this.showEditForm = false; 
  }
  

  closeModal() {
   
    this.showCreateForm = false;
  }

  // ===============================
  // SỬA BÀI KIỂM TRA
  // ===============================
  openEditModal(test: any) {
    this.editTest = {
      id: test.id,
      tenBaiKiemTra: test.tenBaiKiemTra || test.ten,
      ngayKiemTra: this.formatDateToInput(test.ngayKiemTra), // 👈 Quan trọng!
      tenLop: test.tenLop,
      taiLieu: null,
      document: test.urlFile
        ? {
            name: test.urlFile.split('/').pop(),
            url: test.urlFile
          }
        : null
    };
  
    this.onClassSelected(this.editTest.tenLop);
    this.showEditForm = true;
    this.showCreateForm = false;
  }
  formatDateToInput(date: string): string {
    if (!date) return '';
    const d = new Date(date);
    const year = d.getFullYear();
    const month = ('0' + (d.getMonth() + 1)).slice(-2);
    const day = ('0' + d.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }
  

  closeEditModal() {
    this.showEditForm = false;
  }

  updateTest() {
  if (!this.editTest.id || !this.editTest.tenBaiKiemTra || !this.editTest.ngayKiemTra || !this.editTest.tenLop) {
    this.toastr.warning('Vui lòng điền đầy đủ thông tin!');
    return;
  }

  const formData = new FormData();
  formData.append('BaiKiemTraDto.Id', this.editTest.id);
  formData.append('BaiKiemTraDto.TenBaiKiemTra', this.editTest.tenBaiKiemTra);
  formData.append('BaiKiemTraDto.NgayKiemTra', this.editTest.ngayKiemTra);
  formData.append('BaiKiemTraDto.TenLop', this.editTest.tenLop);

  if (this.editTest.taiLieu instanceof File) {
    formData.append('BaiKiemTraDto.TaiLieu', this.editTest.taiLieu);
  }

  this.testlistService.updateTest(formData).subscribe({
    next: (res) => {
      if (res?.isError) {
        this.toastr.error(res.message || res.errors?.[0] || 'Cập nhật thất bại!');
        return;
      }

      this.toastr.success('Cập nhật bài kiểm tra thành công!');
      this.closeEditModal();
      this.loadTests();
    },
    error: (err) => {
      const res = err?.error;
      if (res?.isError) {
        this.toastr.error(res.message || res.errors?.[0] || 'Cập nhật thất bại!');
      } else {
        this.toastr.error('Đã xảy ra lỗi không xác định.');
      }
    }
  });
}

  removeDocument() {
    this.editTest.document = null;
  }
  confirmAndDelete(test: any) {
    const confirmDelete = confirm(`Bạn có chắc chắn muốn xoá bài kiểm tra "${test.tenBaiKiemTra || test.ten}"?`);
    if (!confirmDelete) return;
  
    this.testlistService.deleteTest(test.id).subscribe({
      next: () => {
        this.toastr.success('Xoá bài kiểm tra thành công');
        this.loadTests(); // Refresh danh sách
      },
      error: (err) => {
       
        this.toastr.error('Xoá bài kiểm tra thất bại ');
      }
    });
  }
  
}

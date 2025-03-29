import { Component, OnInit,ChangeDetectorRef  } from '@angular/core';
import { TestlistService } from './shared/testlist.service';
import { BaiKiemTraDto } from './shared/testlist.model';
@Component({
  selector: 'app-testlist',
  templateUrl: './testlist.component.html',
  styleUrls: ['./testlist.component.scss']
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
  constructor(private testlistService: TestlistService,private cdr: ChangeDetectorRef) {}
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
    console.log('➡️ Gửi lên API:', {
      trangThaiToSend,
      tenLop,
      tenBaiKiemTra
    });
  
    this.testlistService.getTests(this.currentPage, this.itemsPerPage, trangThaiToSend, tenLop, tenBaiKiemTra)
      .subscribe({
        next: res => {
          console.log('✅ Kết quả trả về:', res);
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
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.newTest.taiLieu = file;
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
      console.error('❌ Lỗi lấy danh sách lớp:', err);
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
    alert('Vui lòng điền đầy đủ thông tin!');
    return;
  }

  const formData = new FormData();
  formData.append('BaiKiemTraDto.TenBaiKiemTra', this.newTest.tenBaiKiemTra);
  formData.append('BaiKiemTraDto.NgayKiemTra', this.newTest.ngayKiemTra);
  formData.append('BaiKiemTraDto.TenLop', this.newTest.tenLop);
  formData.append('BaiKiemTraDto.TaiLieu', this.newTest.taiLieu);

  this.testlistService.createTest(formData).subscribe({
    next: () => {
      alert('✅ Tạo bài kiểm tra thành công!');
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
    error: (err) => {
      console.error('❌ Tạo bài kiểm tra thất bại:', err);
      alert('Tạo bài kiểm tra thất bại!');
    }
  });
}




  

  openModal() {
    this.showCreateForm = true;
    this.showEditForm = false; // tắt modal sửa khi mở modal tạo
  }
  

  closeModal() {
    console.log('Modal đóng ❌');
    this.showCreateForm = false;
  }

  // ===============================
  // SỬA BÀI KIỂM TRA
  // ===============================
  openEditModal(test: any) {
    this.editTest = { ...test };
    this.showEditForm = true;
    this.showCreateForm = false; // tắt modal tạo khi mở modal sửa
  }

  closeEditModal() {
    this.showEditForm = false;
  }

  updateTest() {
    const index = this.testList.findIndex(t => t === this.editTest);
    if (index > -1) {
      this.testList[index] = {
        ...this.editTest,
        createdDate: this.testList[index].createdDate // giữ nguyên ngày tạo
      };
    }

    this.showEditForm = false;
    this.updatePaginatedList();
  }
  removeDocument() {
    this.editTest.document = null;
  }
}

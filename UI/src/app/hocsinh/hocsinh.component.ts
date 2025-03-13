import { Component, OnInit, HostListener } from '@angular/core';
import { HocSinh } from './shared/hocsinh.model';
import { HocSinhService } from './shared/hocsinh.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-hocsinh',
  templateUrl: './hocsinh.component.html',
  styleUrls: ['./hocsinh.component.scss']
})

export class HocsinhComponent implements OnInit {

  trangThai: string = '';
  lop: string = '';
  searchTerm: string = '';
  
  classOptions = [
    { code: 'Lớp 1', name: 'Lớp 1' },
    { code: 'Lớp 2', name: 'Lớp 2' },
    { code: 'Lớp 3', name: 'Lớp 3' },
    { code: 'Lớp 4', name: 'Lớp 4' },
    { code: 'Lớp 5 Anh', name: 'Lớp 5 Anh' },
  ];
   // Mảng lọc khi tìm kiếm
   filteredClassOptions = this.classOptions.slice();

   // Từ khoá gõ trong ô tìm kiếm
   lopSearchTerm: string = '';
 
   // Trạng thái mở/đóng dropdown lớp
   lopDropdownOpen: boolean = false;
 
   // Khi bấm vào vùng hiển thị "Lớp"
   toggleLopDropdown() {
     this.lopDropdownOpen = !this.lopDropdownOpen;
     if (this.lopDropdownOpen) {
       // Reset tìm kiếm mỗi lần mở dropdown
       this.lopSearchTerm = '';
       this.filteredClassOptions = this.classOptions.slice();
     }
   }
 
   // Khi gõ vào ô "Tìm lớp"
   onLopSearchTermChange() {
     const lower = this.lopSearchTerm.toLowerCase();
     this.filteredClassOptions = this.classOptions.filter(opt =>
       opt.name.toLowerCase().includes(lower) ||
       opt.code.toLowerCase().includes(lower)
     );
   }
 
   // Khi chọn 1 lớp
   selectLop(option: { code: string; name: string }) {
     // Ở đây ta gán lop = code hoặc name, tuỳ bạn muốn hiển thị
     this.lop = option.name; // hoặc `${option.code} - ${option.name}`
     this.lopDropdownOpen = false;
   }
 
   // Lắng nghe click ngoài dropdown => đóng dropdown
   @HostListener('document:click', ['$event'])
   onClickOutside(event: MouseEvent) {
     const path = event.composedPath && event.composedPath();
     // Kiểm tra xem có click vào .lop-select-container hay không
     const clickedInside = path?.some((node: any) =>
       node?.classList?.contains?.('lop-select-container')
     );
     if (!clickedInside) {
       this.lopDropdownOpen = false;
     }
   }

   

  students: (HocSinh & { showDetails: boolean })[] = [
    {
      code: 'HE171450',
      ten: 'Bùi Ngọc Dũng',
      gioiTinh: 'Nam',
      diaChi: '245 Phạm Ngọc Thạch, Đống Đa, TP Hà Nội',
      lop: 'Lớp 1',
      truongDangHoc: 'Trường Đại học FPT',
      ngaySinh: new Date(2003, 6, 23),  // 6 = July
      email: 'dungbnhe17457@fpt.edu.vn',
      soDienThoai: '0123-456-789',
      coSoId: 'cs1',
      coso: {
        id: 'cs1',
        ten: 'Hoàng Văn Thái',
        diaChi: '',
        soDienThoai: '',
        trangThai: '',
        default: false
      },
      chinhSach: 'Cơ bản',
      lopHocs: ['Toán 1', 'Tiếng Anh 1', 'Tiếng Việt 1'],
      showDetails: false
    },
    {
      code: 'HE171466',
      ten: 'Ngô Minh Kiên',
      gioiTinh: 'Nam',
      diaChi: 'Long Biên, Hà Nội',
      lop: 'Lớp 1',
      truongDangHoc: 'Trường Đại học FPT',
      ngaySinh: new Date(2003, 4, 10),  // 4 = May (hiển thị June)
      email: 'kiennmhe17146@fpt.edu.vn',
      soDienThoai: '0987-654-321',
      coSoId: 'cs2',
      coso: {
        id: 'cs2',
        ten: 'Phạm Văn Đồng',
        diaChi: '',
        soDienThoai: '',
        trangThai: '',
        default: false
      },
      chinhSach: 'Nâng cao',
      lopHocs: ['Lớp Toán 2', 'Lớp Anh 2'],
      showDetails: false
    }
  ];

  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  

  onChooseFileclick() {
    this.router.navigate(['/hocsinh/import-hocsinh']);
  }

  onExportFile() {
    alert('Xuất Tệp');
  }

  currentPage: number = 1;
  totalPages: number = 2;

  goToPage(page: number) {
    this.currentPage = page;
  }

  // --- Pop-up "Thêm Học Viên Mới" (các biến mới) ---
  isModalOpen: boolean = false;

  newStudent: any = {
    code: '',
    ten: '',
    gioiTinh: 'Nam',
    ngaySinh: '',
    email: '',
    soDienThoai: '',
    truongDangHoc: '',
    chinhSach: '',
    province: '',
    district: '',
    diaChi: ''
  };

  openAddStudentModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  submitNewStudent() {
    console.log('Thêm học viên mới:', this.newStudent);
    const newHs: HocSinh & { showDetails: boolean } = {
      code: this.newStudent.code,
      ten: this.newStudent.ten,
      gioiTinh: this.newStudent.gioiTinh,
      ngaySinh: this.newStudent.ngaySinh ? new Date(this.newStudent.ngaySinh) : new Date(),
      email: this.newStudent.email,
      soDienThoai: this.newStudent.soDienThoai,
      diaChi: this.newStudent.diaChi,
      lop: '',
      truongDangHoc: this.newStudent.truongDangHoc,
      coSoId: '',
      coso: { id: '', ten: '', diaChi: '', soDienThoai: '', trangThai: '', default: false },
      chinhSach: this.newStudent.chinhSach,
      lopHocs: [],
      showDetails: false
    };
    this.students.push(newHs);
    this.newStudent = {
      code: '',
      ten: '',
      gioiTinh: 'Nam',
      ngaySinh: '',
      email: '',
      soDienThoai: '',
      truongDangHoc: '',
      chinhSach: '',
      province: '',
      district: '',
      diaChi: ''
    };
    this.isModalOpen = false;
  }

  
  provinces: any[] = [];
  districts: any[] = [];

  constructor(private hocSinhService: HocSinhService,private router: Router) {}

  ngOnInit(): void {
    this.hocSinhService.getProvinces().subscribe(
      data => {
        this.provinces = data;
      },
      error => {
        console.error('Error fetching provinces:', error);
      }
    );
  }

  onProvinceChange(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code == provinceCode);
    if (selectedProvince) {
      this.districts = selectedProvince.districts;
    } else {
      this.districts = [];
    }
  }

 
  isEditModalOpen: boolean = false;
  editStudent: any = {
    code: '',
    ten: '',
    gioiTinh: 'Nam',
    ngaySinh: '',
    email: '',
    soDienThoai: '',
    truongDangHoc: '',
    chinhSach: '',
    province: '',
    district: '',
    diaChi: ''
  };
  editDistricts: any[] = [];

  // Khi bấm nút “Sửa” ở bảng
  onEditStudentClick(index: number) {
    const hs = this.students[index];
    // Copy data sang editStudent
    this.editStudent = {
      code: hs.code,
      ten: hs.ten,
      gioiTinh: hs.gioiTinh,
      ngaySinh: hs.ngaySinh ? this.formatDate(hs.ngaySinh) : '',
      email: hs.email,
      soDienThoai: hs.soDienThoai,
      truongDangHoc: hs.truongDangHoc,
      chinhSach: hs.chinhSach,
      province: hs.province || '',
      district: hs.district || '',
      diaChi: hs.diaChi
    };
    // Cập nhật quận/huyện khi province thay đổi
    this.onProvinceChangeForEdit(this.editStudent.province);

    this.isEditModalOpen = true;
  }
  closeEditModal() {
    this.isEditModalOpen = false;
  }
  submitEditStudent() {
    // Cập nhật data vào mảng students
    const idx = this.students.findIndex(h => h.code === this.editStudent.code);
    if (idx !== -1) {
      const updatedHs = this.students[idx];
      updatedHs.ten = this.editStudent.ten;
      updatedHs.gioiTinh = this.editStudent.gioiTinh;
      updatedHs.ngaySinh = this.editStudent.ngaySinh ? new Date(this.editStudent.ngaySinh) : null;
      updatedHs.email = this.editStudent.email;
      updatedHs.soDienThoai = this.editStudent.soDienThoai;
      updatedHs.truongDangHoc = this.editStudent.truongDangHoc;
      updatedHs.chinhSach = this.editStudent.chinhSach;
      updatedHs.province = this.editStudent.province;
      updatedHs.district = this.editStudent.district;
      updatedHs.diaChi = this.editStudent.diaChi;
    }
    this.isEditModalOpen = false;
  }
  onProvinceChangeForEdit(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code === provinceCode);
    this.editDistricts = selectedProvince ? selectedProvince.districts : [];
  }

  // Hàm format date => yyyy-MM-dd
  formatDate(date: Date) {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
  
}

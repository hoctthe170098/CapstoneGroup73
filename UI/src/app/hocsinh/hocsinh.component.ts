import { Component, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
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
   filteredClassOptions = this.classOptions.slice();

   lopSearchTerm: string = '';
 
   lopDropdownOpen: boolean = false;

   students: (HocSinh & { showDetails: boolean })[] = [];
  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];
 // Phân trang
 currentPage: number = 1;
 totalPages: number = 1;
 pageSize: number = 8;
 totalItems: number = 0;

 // Biến xử lý modal
 isModalOpen: boolean = false;
 isEditModalOpen: boolean = false;
 newStudent: any = {};
 editStudent: any = {};

 
   toggleLopDropdown() {
     this.lopDropdownOpen = !this.lopDropdownOpen;
     if (this.lopDropdownOpen) {
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

   

  

  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  

  onChooseFileclick() {
    this.router.navigate(['/hocsinh/import-hocsinh']);
  }

  onExportFile() {
    alert('Xuất Tệp');
  }



  goToPage(page: number) {
    this.currentPage = page;
  }

  

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

  


  constructor(private hocSinhService: HocSinhService,private router: Router,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadDanhSachHocSinh();
    this.loadProvinces();
  }

  onProvinceChange(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code == provinceCode);
    if (selectedProvince) {
      this.districts = selectedProvince.districts;
    } else {
      this.districts = [];
    }
  }
  loadDanhSachHocSinh() {
    let isActiveFilter: boolean | null = this.trangThai === 'Hoạt động' ? true : this.trangThai === 'Tạm ngừng' ? false : null;

    this.hocSinhService.getDanhSachHocSinh(this.currentPage, this.pageSize, this.searchTerm, '', isActiveFilter, this.lop)
      .subscribe(response => {
        console.log("📌 API Response:", response);
        if (!response.isError && response.data) {
          this.students = response.data.map((hs: any) => ({
            code: hs.code || '',
            ten: hs.ten || '',
            gioiTinh: hs.gioiTinh || '',
            diaChi: hs.diaChi || '',
            lop: hs.lop || '',
            truongDangHoc: hs.truongDangHoc || '',
            ngaySinh: hs.ngaySinh ? new Date(hs.ngaySinh) : null,
            email: hs.email || '',
            soDienThoai: hs.soDienThoai || '',
            isActive: hs.isActive !== undefined ? hs.isActive : false,
            tenCoSo: hs.tenCoSo || '',
            chinhSach: hs.tenChinhSach || '',
            lopHocs: hs.tenLops ? hs.tenLops : [],
            showDetails: false
          }));

          this.totalItems = response.data.totalCount || this.students.length;
          this.totalPages = Math.ceil(this.totalItems / this.pageSize);
        } else {
          this.students = [];
          this.totalItems = 0;
          this.totalPages = 0;
        }
        this.cdr.detectChanges();
      });
  }
  /** 🔄 Chuyển trang */
  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadDanhSachHocSinh();
    }
  }

  /** 🔍 Tìm kiếm theo lớp */
  selectLop(option: { code: string; name: string }) {
    this.lop = option.name;
    this.lopDropdownOpen = false;
    this.loadDanhSachHocSinh();
  }

  /** 🗺️ Lấy danh sách tỉnh/thành phố */
  loadProvinces() {
    this.hocSinhService.getProvinces().subscribe(
      data => {
        this.provinces = data;
      },
      error => {
        console.error('Lỗi tải tỉnh/thành phố:', error);
      }
    );
  }
 
  

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

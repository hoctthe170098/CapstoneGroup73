import { Component, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { HocSinh } from './shared/hocsinh.model';
import { HocSinhService } from './shared/hocsinh.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
 pageSize: number = 8;
 totalPages: number = 1;
 totalItems: number = 0;

 // Biến xử lý modal
 addStudentForm: FormGroup;
 isModalOpen: boolean = false;
 isEditModalOpen: boolean = false;
 newStudent: any = {};
 editStudent: any = {};
 policies: any[] = []; 

 
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

   

  

   searchHocSinh() {
    this.currentPage = 1;
    this.loadDanhSachHocSinh();
  }

  /** Mở rộng chi tiết */
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
    this.addStudentForm.reset();
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  submitNewStudent() {
    console.log("🚀 Hàm submitNewStudent() được gọi!");
  
    if (this.addStudentForm.invalid) {
      console.log("❌ Form không hợp lệ", this.addStudentForm.errors);
      this.addStudentForm.markAllAsTouched();
      return;
    }
  
    const formData = this.addStudentForm.value;
  
    // ✅ Xử lý địa chỉ đầy đủ
    const provinceObj = this.provinces.find(p => p.code == formData.province);
    const provinceName = provinceObj ? provinceObj.name : '';
  
    const districtObj = this.districts.find(d => d.code == formData.district);
    const districtName = districtObj ? districtObj.name : '';
  
    const diaChiFormatted = `${provinceName}, ${districtName}, ${formData.diaChiCuThe}`;
  
    // ✅ Xử lý chính sách học phí, nếu "Không chọn" thì đặt là `null` hoặc loại bỏ hoàn toàn
    let selectedPolicy = formData.chinhSachId;
    if (!selectedPolicy || selectedPolicy === "" || selectedPolicy === "-- Không chọn --") {
      selectedPolicy = null; // Có thể thử null hoặc không gửi key này đi
    }
  
    const newStudent = {
      code: formData.code,
      ten: formData.ten,
      gioiTinh: formData.gioiTinh,
      ngaySinh: formData.ngaySinh,
      email: formData.email,
      soDienThoai: formData.soDienThoai,
      truongDangHoc: formData.truongDangHoc,
      lop: formData.lop,
      diaChi: diaChiFormatted,
      ...(selectedPolicy !== null && { chinhSachId: selectedPolicy }) // 🔹 Chỉ thêm nếu không phải null
    };
  
    console.log("📤 Gửi API thêm học sinh:", newStudent);
  
    this.hocSinhService.createHocSinh(newStudent).subscribe({
      next: (res) => {
        console.log("📌 Phản hồi từ API:", res);
        if (!res.isError) {
          this.toastr.success("Thêm học sinh thành công!", "Thành công");
          this.closeModal();
          this.loadDanhSachHocSinh();
        } else {
          this.toastr.error(res.message, "Lỗi");
        }
      },
      error: (err) => {
        console.error("❌ Lỗi khi gọi API:", err);
        this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
      }
    });
  }
  


  constructor(private hocSinhService: HocSinhService,private router: Router,private cdr: ChangeDetectorRef, 
    private toastr: ToastrService,
    private fb: FormBuilder) {
      // Form thêm học sinh
    this.addStudentForm = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(18)]],
      ten: ['', [Validators.required, Validators.maxLength(50)]],
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]],
      truongDangHoc: ['', Validators.required],
      lop: ['', Validators.required], 
      province: ['', Validators.required],  // Thành phố
      district: ['', Validators.required],  // Quận/Huyện
      diaChiCuThe: ['', Validators.required], // Địa chỉ cụ thể
      chinhSachId: ['']
    });
    }

  ngOnInit(): void {
    this.loadDanhSachHocSinh();
    this.loadProvinces();
    this.loadDanhSachChinhSach();
  }

  loadDanhSachChinhSach() {
    this.hocSinhService.getDanhSachChinhSach().subscribe(
      response => {
        if (!response.isError && response.data) {
          this.policies = response.data; // Gán dữ liệu vào biến policies
          console.log("📌 Danh sách chính sách:", this.policies);
        } else {
          this.policies = [];
          console.error("Lỗi tải danh sách chính sách!");
        }
      },
      error => {
        console.error("❌ Lỗi khi gọi API danh sách chính sách:", error);
      }
    );
  }

  onProvinceChange(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code == provinceCode);
    this.districts = selectedProvince ? selectedProvince.districts : [];
  }
  loadDanhSachHocSinh() {
    let isActiveFilter: boolean | null = this.trangThai === 'Hoạt động' ? true : this.trangThai === 'Tạm ngừng' ? false : null;

    this.hocSinhService.getDanhSachHocSinh(1, 9999, this.searchTerm, '', isActiveFilter, '')
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
           
            chinhSach: hs.tenChinhSach && hs.tenChinhSach.trim() !== '' ? hs.tenChinhSach : 'Cơ bản', 
            lopHocs: hs.tenLops ? hs.tenLops : [],
            showDetails: false
          }));

          this.totalItems = response.totalRecords ?? response.totalCount ?? this.students.length;
          this.totalPages = Math.ceil(this.totalItems / this.pageSize);
          console.log("📌 API Response:", response);
console.log("📌 Total Records:", response.totalRecords);
console.log("📌 Students Length:", this.students.length);

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
  get paginatedStudents() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    return this.students.slice(startIndex, endIndex);
  }
  filterByStatus() {
    this.currentPage = 1; // Reset về trang đầu tiên khi lọc
    this.loadDanhSachHocSinh();
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
        console.error('Lỗi tải danh sách tỉnh/thành phố:', error);
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

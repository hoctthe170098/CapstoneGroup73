import { Component, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { HocSinh } from './shared/hocsinh.model';
import { HocSinhService } from './shared/hocsinh.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { saveAs } from 'file-saver';
import { NgxSpinnerService } from 'ngx-spinner';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-hocsinh',
  templateUrl: './hocsinh.component.html',
  styleUrls: ['./hocsinh.component.scss']
})

export class HocsinhComponent implements OnInit {

  trangThai: string = '';
  lop: string = '';
  searchTerm: string = '';
  
  classOptions: { code: string; name: string }[] = [];
filteredClassOptions: { code: string; name: string }[] = [];
private lopSearchSubject = new Subject<string>();


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
 newStudent: any = {};
 editStudent: any = {};
 policies: any[] = []; 

editStudentForm: FormGroup;
selectedStudent: any;
isEditModalOpen: boolean = false;
 
   
 
   // Khi gõ vào ô "Tìm lớp"
   onLopSearchTermChange() {
    this.getDanhSachLopHoc(this.lopSearchTerm.trim());
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
  toggleLopDropdown() {
    this.lopDropdownOpen = !this.lopDropdownOpen;
  
    if (this.lopDropdownOpen) {
      this.lopSearchTerm = '';
      this.getDanhSachLopHoc(''); // Gọi API để load tất cả lớp
    }
  }
  getDanhSachLopHoc(keyword: string) {
    this.hocSinhService.getDanhSachLopTheoTen(keyword).subscribe(res => {
      if (!res.isError && res.data) {
        this.classOptions = res.data.map((item: any) => ({
          code: item.tenLop,
          name: item.tenLop
        }));
        this.filteredClassOptions = this.classOptions;
      } else {
        this.classOptions = [];
        this.filteredClassOptions = [];
      }
    });
  }
  
  
  

  onChooseFileclick() {
    this.router.navigate(['/hocsinh/import-hocsinh']);
  }

  onExportFile() {
    const isActiveFilter: boolean | null = this.trangThai === 'Hoạt động' ? true : this.trangThai === 'Tạm ngừng' ? false : null;
  
    // Gọi lại API với pageSize rất lớn để lấy toàn bộ danh sách học sinh theo filter hiện tại
    this.hocSinhService.getDanhSachHocSinh(1, 10000, this.searchTerm, '', isActiveFilter, '')
      .subscribe({
        next: (response) => {
          if (!response.isError && response.data && response.data.items.length > 0) {
            const payload = response.data.items.map((hs: any) => ({
              code: hs.code || '',
              ten: hs.ten || '',
              gioiTinh: hs.gioiTinh || '',
              diaChi: hs.diaChi || '',
              lop: hs.lop || '',
              truongDangHoc: hs.truongDangHoc || '',
              ngaySinh: hs.ngaySinh ? this.formatDate(hs.ngaySinh) : '',
              email: hs.email || '',
              soDienThoai: hs.soDienThoai || '',
              tenCoSo: hs.tenCoSo || '',
              tenChinhSach: hs.tenChinhSach || null,
              userId: hs.userId || '',
              isActive: hs.isActive !== undefined ? hs.isActive : true,
              tenLops: hs.tenLops || []
            }));
  
            this.hocSinhService.exportHocSinhsToExcel(payload).subscribe({
              next: (blob: Blob) => {
                const fileName = 'DanhSachHocSinh.xlsx';
                saveAs(blob, fileName);
                this.toastr.success("Xuất tệp thành công!", "Thành công");
              },
              error: () => {
                this.toastr.error("Xuất tệp thất bại!", "Lỗi");
              }
            });
          } else {
            this.toastr.warning("Không có học sinh để xuất!", "Cảnh báo");
          }
        },
        error: () => {
          this.toastr.error("Lỗi khi lấy dữ liệu học sinh!", "Lỗi");
        }
      });
  }
  



  goToPage(page: number) {
    this.currentPage = page;
  }

  

  openAddStudentModal() {
    this.addStudentForm.reset({
      gioiTinh: 'Nam'  
    });
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  submitNewStudent() {
    if (this.addStudentForm.invalid) {
      this.addStudentForm.markAllAsTouched();
      return;
    }
  
    const formData = this.addStudentForm.value;
  
  
  
    const provinceObj = this.provinces.find(p => String(p.code) === String(formData.province));
    const provinceName = provinceObj?.name || '';
  
    const districtObj = this.districts.find(d => String(d.code) === String(formData.district));
    const districtName = districtObj?.name || '';
  
    const chiTiet = formData.diaChiCuThe || '';
  
    const diaChiFormatted = `${provinceName}, ${districtName}, ${chiTiet}`.trim();
  
    let chinhSachId = formData.chinhSachId || null;
    if (chinhSachId === '') chinhSachId = null;
  
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
      ...(chinhSachId !== null && { chinhSachId })
    };
  
    this.spinner.show();
    this.hocSinhService.createHocSinh(newStudent).subscribe({
      next: (res) => {
        this.spinner.hide();
        if (!res.isError) {
          this.toastr.success("Thêm học sinh thành công!", "Thành công");
          this.closeModal();
          this.loadDanhSachHocSinh();
        } else {
          
          this.toastr.error(res.message, "Lỗi");
        }
      },
      error: (err) => {
        this.spinner.hide();
        console.error(" Lỗi khi gọi API:", err);
        this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
      }
    });
  }
  constructor(private hocSinhService: HocSinhService,private router: Router,private cdr: ChangeDetectorRef, 
    private toastr: ToastrService,
    private fb: FormBuilder,
    private spinner:NgxSpinnerService                        ) {
      // Form thêm học sinh
    this.addStudentForm = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(18)]],
      ten: ['', [Validators.required, Validators.maxLength(50)]],
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]],
      truongDangHoc: ['', Validators.required, Validators.maxLength(50)],
      lop: ['', Validators.required, Validators.maxLength(20)], 
      province: ['', Validators.required], 
      district: ['', Validators.required],  
      diaChiCuThe: ['', Validators.required], 
      chinhSachId: ['']
    });
    this.editStudentForm = this.fb.group({
      code: [''], 
      ten: ['', [Validators.required, Validators.maxLength(50)]],
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]],
      truongDangHoc: ['', [Validators.required, Validators.maxLength(50)]],
      lop: ['', Validators.required],
      province: ['', Validators.required],
      district: ['', Validators.required],
      diaChiCuThe: ['', [Validators.required, Validators.maxLength(150)]],
      chinhSachId: [''],
      status: [true] 
  });
    }

  ngOnInit(): void {
    this.getDanhSachLopHoc('');
    this.lopSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(search => {
      this.hocSinhService.getDanhSachLopTheoTen(search).subscribe(res => {
        if (!res.isError && res.data) {
          this.classOptions = res.data.map((item: any) => ({
            code: item.tenLop,
            name: item.tenLop
          }));
          this.filteredClassOptions = this.classOptions;
        } else {
          this.classOptions = [];
          this.filteredClassOptions = [];
        }
      });
    });
    
    this.loadDanhSachHocSinh();
    this.loadProvinces();
    this.loadDanhSachChinhSach();
  }

  loadDanhSachChinhSach() {
    this.hocSinhService.getDanhSachChinhSach().subscribe(
      response => {
        if (!response.isError && response.data) {
          this.policies = response.data; // Gán dữ liệu vào biến policies
        } else {
          this.policies = [];
          console.error("Lỗi tải danh sách chính sách!");
        }
      },
      error => {
        console.error(" Lỗi khi gọi API danh sách chính sách:", error);
      }
    );
  }

  onProvinceChange(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code == provinceCode);
    this.districts = selectedProvince ? selectedProvince.districts : [];
  }

  extractAddressParts(diaChi: string): { province: string, district: string, detail: string } {
    const parts = diaChi.split(',').map(part => part.trim());
    return {
      province: parts[0] || '',
      district: parts[1] || '',
      detail: parts.slice(2).join(', ') || ''
    };
  }
  loadDanhSachHocSinh() {
    let isActiveFilter: boolean | null = this.trangThai === 'Hoạt động' ? true : this.trangThai === 'Tạm ngừng' ? false : null;

    this.hocSinhService.getDanhSachHocSinh(this.currentPage, this.pageSize, this.searchTerm, '', isActiveFilter, '')
      .subscribe(response => {

        if (!response.isError && response.data && response.data.items) {
          this.students = response.data.items.map((hs: any) => ({
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

          this.totalItems = response.data.totalCount || 0;
          this.totalPages = Math.ceil(this.totalItems / this.pageSize);


          this.cdr.detectChanges();
        } else {
          this.students = [];
          this.totalItems = 0;
          this.totalPages = 1;
        }
      });
}

openEditStudentModal(student: any) {

  if (!student || !student.code) {
    this.toastr.error("Không tìm thấy mã học sinh!", "Lỗi");
    console.error(" Lỗi: Học sinh không có mã!", student);
    return;
  }

  this.selectedStudent = { ...student };

  const addressParts = student.diaChi ? student.diaChi.split(',').map(part => part.trim()) : ['', '', ''];
  const provinceName = addressParts[0] || '';
  const districtName = addressParts[1] || '';
  const detailAddress = addressParts[2] || '';

  const provinceObj = this.provinces.find(p => p.name === provinceName);
  const provinceCode = provinceObj ? provinceObj.code : '';

  if (!provinceObj) {
    console.warn("⚠️ Không tìm thấy tỉnh/thành phố trong danh sách Edit!", provinceName);
  }

  this.onProvinceChangeForEdit(provinceCode);

  const districtObj = provinceObj?.districts.find(d => d.name === districtName);
  const districtCode = districtObj ? districtObj.code : '';

  const ngaySinhFormatted = student.ngaySinh ? new Date(student.ngaySinh).toISOString().split('T')[0] : '';

  let policyId = this.policies.find(p => p.ten === student.chinhSach)?.id || '';

  //  Gán dữ liệu vào FormGroup
  this.editStudentForm.patchValue({
      code: student.code || '',  
      ten: student.ten,
      gioiTinh: student.gioiTinh,
      ngaySinh: ngaySinhFormatted,
      email: student.email,
      soDienThoai: student.soDienThoai,
      truongDangHoc: student.truongDangHoc,
      lop: student.lop,
      chinhSachId: policyId,
      province: provinceCode,
      district: districtCode,
      diaChiCuThe: detailAddress,
      status: [true]
  });

  
  this.isEditModalOpen = true;
}





  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
        this.currentPage = page;
        this.loadDanhSachHocSinh();  
    }
}

  
  
  filterByStatus() {
    this.currentPage = 1; 
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
 
  

  onEditStudentClick(index: number) { 
    const hs = this.students[index];
  
  
    if (!hs || !hs.code) {
      this.toastr.error("Không tìm thấy mã học sinh!", "Lỗi");
      console.error(" Lỗi: Học sinh không có mã!", hs);
      return;
    }
  
    const addressParts = hs.diaChi ? hs.diaChi.split(',').map(p => p.trim()) : ['', '', ''];
    const [provinceName, districtName, detailAddress] = addressParts;
  
    const provinceObj = this.provinces.find(p => p.name === provinceName);
    const provinceCode = provinceObj ? provinceObj.code : '';
    this.onProvinceChangeForEdit(provinceCode);
  
    const districtObj = provinceObj?.districts.find(d => d.name === districtName);
    const districtCode = districtObj ? districtObj.code : '';
  
    const policyObj = this.policies.find(p => p.ten === hs.chinhSach);
    const policyId = policyObj ? policyObj.id : '';
  
  
    this.editStudentForm.patchValue({
      code: hs.code || '', 
      ten: hs.ten,
      gioiTinh: hs.gioiTinh,
      ngaySinh: hs.ngaySinh ? new Date(hs.ngaySinh).toISOString().split('T')[0] : '',
      email: hs.email,
      soDienThoai: hs.soDienThoai,
      truongDangHoc: hs.truongDangHoc,
      lop: hs.lop,
      chinhSachId: policyId,
      province: provinceCode,
      district: districtCode,
      diaChiCuThe: detailAddress,
      
    });
  
  
    this.isEditModalOpen = true;
  }
  


  closeEditModal() {
    this.isEditModalOpen = false;
  }
  submitEditStudent() {
    if (this.editStudentForm.invalid) {
        this.toastr.error("Vui lòng nhập đầy đủ thông tin!", "Lỗi");
        return;
    }

    let formData = { ...this.editStudentForm.value };


    if (!formData.code || formData.code.trim() === "") {
        this.toastr.error("Không tìm thấy mã học sinh!", "Lỗi");
        console.error(" Lỗi: Mã học sinh không tồn tại trong form!", formData);
        return;
    }

    if (formData.ngaySinh) {
        const date = new Date(formData.ngaySinh);
        formData.ngaySinh = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
    }

    const provinceObj = this.provinces.find(p => p.code == formData.province);
    const provinceName = provinceObj ? provinceObj.name : '';

    const districtObj = this.editDistricts.find(d => d.code == formData.district);
    const districtName = districtObj ? districtObj.name : '';

    formData.diaChi = `${provinceName}, ${districtName}, ${formData.diaChiCuThe || ''}`.trim();

    formData.status = formData.status ? "true" : "false";

    if (formData.chinhSachId) {
        formData.chinhSachId = String(formData.chinhSachId);
    }

    const filteredData = {
        code: formData.code,
        ten: formData.ten,
        gioiTinh: formData.gioiTinh,
        diaChi: formData.diaChi, 
        lop: formData.lop,
        truongDangHoc: formData.truongDangHoc,
        ngaySinh: formData.ngaySinh,
        email: formData.email,
        soDienThoai: formData.soDienThoai,
        chinhSachId: formData.chinhSachId,
        status: formData.status
    };


    this.hocSinhService.updateHocSinh(filteredData).subscribe({
        next: (res) => {
            if (!res.isError) {
                this.toastr.success("Cập nhật học sinh thành công!", "Thành công");
                this.closeEditModal();
                this.loadDanhSachHocSinh();
            } else {
                this.toastr.error(res.message, "Lỗi");
            }
        },
        error: (error) => {
            console.error(" Lỗi kết nối API:", error);
            this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
        }
    });
}


  
  


onProvinceChangeForEdit(provinceCode: string) {

  const selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));

  if (selectedProvince) {
      this.editDistricts = selectedProvince.districts;
  } else {
      console.warn("Không tìm thấy tỉnh/thành phố trong danh sách Edit!");
      this.editDistricts = [];
  }

 
}



  // Hàm format date => yyyy-MM-dd
  formatDate(date: Date) {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
  clearSelectedLop(event: MouseEvent) {
    event.stopPropagation(); 
    this.lop = '';
    this.loadDanhSachHocSinh(); 
  }
}

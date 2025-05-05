import { Component, OnInit, ChangeDetectorRef  } from '@angular/core';
import { Giaovien } from './shared/giaovien.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GiaovienService } from './shared/giaovien.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { saveAs } from 'file-saver';
import { NgxSpinnerService } from 'ngx-spinner';
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
   totalItems: number = 0;

  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];
  addTeacherForm: FormGroup;
  editTeacherForm: FormGroup;
  selectedTeacher: any;

  constructor(private giaovienService: GiaovienService, private router: Router, private cdr: ChangeDetectorRef, private toastr: ToastrService, private fb: FormBuilder, private spinner: NgxSpinnerService) {
    this.addTeacherForm = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(18)]], 
      ten: ['', [Validators.required, Validators.maxLength(20)]], 
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', [Validators.required, this.validateAge]], 
      email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]], 
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]], 
      truongDangDay: ['', [Validators.required, Validators.maxLength(50)]], 
      province: ['', Validators.required],
      district: ['', Validators.required],
      diaChiCuThe: ['', [Validators.required, Validators.maxLength(150)]], 
      status: [true]
    });
  
    this.editTeacherForm = this.fb.group({
      code: [''],
      ten: ['', [Validators.required, Validators.maxLength(20)]],
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', [Validators.required, this.validateAge]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]],
      truongDangDay: ['', [Validators.required, Validators.maxLength(50)]],
      province: ['', Validators.required],
      district: ['', Validators.required],
      diaChiCuThe: ['', [Validators.required, Validators.maxLength(150)]],
      status: [true]
    });
  }

  ngOnInit(): void {
    this.loadDanhSachGiaoVien();
    this.loadProvinces();
  }

  loadProvinces() {
    this.giaovienService.getProvinces().subscribe(
        data => {
            this.provinces = data;
        },
        error => {
            console.error('Lỗi tải tỉnh/thành phố:', error);
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
  this.addTeacherForm.patchValue({ district: '' }); 
    
}

checkEmailExists(email: string): Promise<boolean> {
  return new Promise((resolve) => {
    this.giaovienService.getDanhSachGiaoVien().subscribe((response) => {
      if (!response.isError && response.data && response.data.items) {
        const existingEmail = response.data.items.some((gv: any) => gv.email === email);
        resolve(existingEmail);
      } else {
        resolve(false);
      }
    });
  });
}

checkPhoneExists(phone: string): Promise<boolean> {
  return new Promise((resolve) => {
    this.giaovienService.getDanhSachGiaoVien().subscribe((response) => {
      if (!response.isError && response.data && response.data.items) {
        const existingPhone = response.data.items.some((gv: any) => gv.soDienThoai === phone);
        resolve(existingPhone);
      } else {
        resolve(false);
      }
    });
  });
}



onProvinceChangeForEdit(provinceCode: string) {
  const selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));
  this.editDistricts = selectedProvince ? selectedProvince.districts : [];

  const districtControl = this.editTeacherForm.get('district');
  districtControl?.setValue('');
  districtControl?.markAsTouched();
  districtControl?.updateValueAndValidity();
}

  
loadDanhSachGiaoVien() {
  let isActiveFilter: boolean | null = this.trangThai === 'Hoạt động' ? true : this.trangThai === 'Tạm ngừng' ? false : null;

  this.giaovienService.getDanhSachGiaoVien(this.currentPage, this.pageSize, this.searchTerm, '', isActiveFilter)
    .subscribe(response => {
      if (response.code === 404) {
        this.router.navigate(['/pages/error'])
        return;
      }

      if (!response.isError && response.data && response.data.items) {
        this.students = response.data.items.map((gv: any) => ({
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

changePage(page: number) {
  if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadDanhSachGiaoVien(); 
  }
}

  

  searchGiaoVien() {
    this.currentPage = 1;
    this.loadDanhSachGiaoVien();
  }

  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  openEditTeacherModal(teacher: any) {
  
    this.selectedTeacher = { ...teacher };
  
    if (!teacher.code || teacher.code.trim() === "") {
      this.toastr.error("Giáo viên không có mã, không thể chỉnh sửa!", "Lỗi");
      return;
    }
  
    this.editTeacherForm.patchValue({
      code: teacher.code,
      ten: teacher.ten,
      gioiTinh: teacher.gioiTinh,
      ngaySinh: teacher.ngaySinh ? new Date(teacher.ngaySinh).toISOString().split('T')[0] : '',
      email: teacher.email,
      soDienThoai: teacher.soDienThoai,
      truongDangDay: teacher.truongDangDay,
      province: teacher.province,
      district: teacher.district,
      diaChiCuThe: teacher.diaChiCuThe,
      status: teacher.isActive 
    });
  
  
    this.isEditModalOpen = true;
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
    province: '', // Lưu mã tỉnh/thành phố
    district: '', // Lưu mã quận/huyện
    diaChiCuThe: '' // Lưu địa chỉ cụ thể
  };
  

  openAddTeacherModal() {
    this.addTeacherForm.reset({
      gioiTinh: 'Nam'  
    });
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  submitNewTeacher() {
    if (this.addTeacherForm.invalid) {
      this.addTeacherForm.markAllAsTouched();
      return;
    }
  
    const formData = this.addTeacherForm.value;
  
    this.spinner.show();
  
    // Gọi check email và phone tuần tự
    this.checkEmailExists(formData.email).then(emailExists => {
      if (emailExists) {
        this.spinner.hide();
        this.toastr.error("Email đã tồn tại!", "Lỗi");
        return;
      }
  
      this.checkPhoneExists(formData.soDienThoai).then(phoneExists => {
        if (phoneExists) {
          this.spinner.hide();
          this.toastr.error("Số điện thoại đã tồn tại!", "Lỗi");
          return;
        }
  
        // Format lại địa chỉ
        const provinceObj = this.provinces.find(p => p.code == formData.province);
        const provinceName = provinceObj ? provinceObj.name : '';
  
        const districtObj = this.districts.find(d => d.code == formData.district);
        const districtName = districtObj ? districtObj.name : '';
  
        const diaChiFormatted = `${provinceName}, ${districtName}, ${formData.diaChiCuThe || ''}`.trim();
  
        const newTeacher = {
          code: formData.code,
          ten: formData.ten,
          gioiTinh: formData.gioiTinh,
          ngaySinh: formData.ngaySinh,
          email: formData.email,
          soDienThoai: formData.soDienThoai,
          truongDangDay: formData.truongDangDay,
          diaChi: diaChiFormatted
        };
  
        // Gọi API tạo giáo viên
        this.giaovienService.createGiaoVien(newTeacher).subscribe({
          next: (res) => {
            this.spinner.hide();
            if (!res.isError) {
              this.toastr.success("Thêm giáo viên thành công!", "Thành công");
              this.closeModal();
              this.loadDanhSachGiaoVien();
            } else {
              this.toastr.error(res.message, "Lỗi");
            }
          },
          error: () => {
            this.spinner.hide();
            this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
          }
        });
      });
    });
  }
  
  

  
  
  getProvinceName(provinceCode: string): string {
  const province = this.provinces.find(p => p.code === provinceCode);
  return province ? province.name : "";
}

getDistrictName(districtCode: string): string {
  const district = this.districts.find(d => d.code === districtCode);
  return district ? district.name : "";
}

  
  
  resetNewStudent() {
    this.newStudent = {
      code: '',
      ten: '',
      gioiTinh: 'Nam',
      ngaySinh: '',
      email: '',
      soDienThoai: '',
      truongDangDay: '',
      province: '',
      district: '',
      diaChi: ''
    };
    this.districts = []; 
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
    const teacher = this.students[index];
    if (!teacher || !teacher.code) {
        this.toastr.error("Không tìm thấy mã giáo viên!", "Lỗi");
        return;
    }

    this.selectedTeacher = { ...teacher }; 


    
    const addressParts = teacher.diaChi ? teacher.diaChi.split(',').map(part => part.trim()) : ['', '', ''];
    const provinceName = addressParts[0] || ''; 
    const districtName = addressParts[1] || '';
    const detailAddress = addressParts[2] || ''; 
    
    
    const provinceObj = this.provinces.find(p => p.name === provinceName);
    const provinceCode = provinceObj ? provinceObj.code : '';

    this.onProvinceChangeForEdit(provinceCode);

    //  Lấy `districtCode` từ `districtName`
    const districtObj = provinceObj?.districts.find(d => d.name === districtName);
    const districtCode = districtObj ? districtObj.code : '';

    const ngaySinhFormatted = teacher.ngaySinh ? new Date(teacher.ngaySinh).toISOString().split('T')[0] : '';

    //  Cập nhật form với dữ liệu chỉnh sửa
    this.editTeacherForm.patchValue({
        code: teacher.code, 
        ten: teacher.ten,
        gioiTinh: teacher.gioiTinh,
        ngaySinh: ngaySinhFormatted,
        email: teacher.email,
        soDienThoai: teacher.soDienThoai,
        truongDangDay: teacher.truongDangDay,
        province: provinceCode,
        district: districtCode,
        diaChiCuThe: detailAddress,
        status: teacher.isActive ? true : false
    });

    this.isEditModalOpen = true;
}


  
  /** Tách diaChi thành tỉnh, quận/huyện, địa chỉ cụ thể */
  extractAddressParts(diaChi: string): { province: string, district: string, detail: string } {
    const parts = diaChi.split(',').map(part => part.trim());
    return {
      province: parts[0] || '',
      district: parts[1] || '',
      detail: parts.slice(2).join(', ') || ''
    };
  }
  
  
  

  closeEditModal() {
    this.isEditModalOpen = false;
  }

  submitEditStudent() {
    const formData = { ...this.editTeacherForm.value };
  
    if (!formData.code || formData.code.trim() === "") {
      if (this.selectedTeacher && this.selectedTeacher.code) {
        formData.code = this.selectedTeacher.code;
      } else {
        this.toastr.error("Không tìm thấy mã giáo viên!", "Lỗi");
        console.error("Lỗi: Mã giáo viên không tồn tại!");
        return;
      }
    }
  
    this.checkEmailExists(formData.email).then(emailExists => {
      if (emailExists && formData.email !== this.selectedTeacher.email) {
        this.toastr.error("Email đã tồn tại!", "Lỗi");
        return;
      }
  
      this.checkPhoneExists(formData.soDienThoai).then(phoneExists => {
        if (phoneExists && formData.soDienThoai !== this.selectedTeacher.soDienThoai) {
          this.toastr.error("Số điện thoại đã tồn tại!", "Lỗi");
          return;
        }
  
        // Format ngày sinh
        if (formData.ngaySinh) {
          const date = new Date(formData.ngaySinh);
          formData.ngaySinh = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
        }
  
        // Địa chỉ
        const provinceObj = this.provinces.find(p => p.code == formData.province);
        const provinceName = provinceObj ? provinceObj.name : '';
  
        const districtObj = this.editDistricts.find(d => d.code == formData.district);
        const districtName = districtObj ? districtObj.name : '';
  
        formData.diaChi = `${provinceName}, ${districtName}, ${formData.diaChiCuThe || ''}`.trim();
        formData.status = formData.status ? "true" : "false";
  
        if (this.editTeacherForm.invalid) {
          this.editTeacherForm.markAllAsTouched();
          return;
        }
  
        this.giaovienService.updateGiaoVien(formData).subscribe({
          next: (res) => {
            if (!res.isError) {
              this.toastr.success("Cập nhật giáo viên thành công!", "Thành công");
              this.closeEditModal();
              this.loadDanhSachGiaoVien();
            } else {
              this.toastr.error(res.message, "Lỗi");
            }
          },
          error: (error) => {
            console.error("Lỗi kết nối API:", error);
            this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
          }
        });
      });
    });
  }
  

validateAge(control: any) {
  if (!control.value) return null;
  const birthDate = new Date(control.value);
  const today = new Date();
  const age = today.getFullYear() - birthDate.getFullYear();
  if (age < 18) {
    return { invalidAge: true };
  }
  return null;
}

  

  filterByStatus() {
    this.currentPage = 1;
    this.loadDanhSachGiaoVien();
  }
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
  
    
    exportGiaoVien() {
      this.giaovienService.exportGiaoViensToExcel().subscribe(
        (response: Blob) => {
          const fileName = 'DanhSachGiaoVien.xlsx';
          saveAs(response, fileName);
        },
        (error) => {
          console.error(' Lỗi khi xuất file:', error);
        }
      );
    }

}

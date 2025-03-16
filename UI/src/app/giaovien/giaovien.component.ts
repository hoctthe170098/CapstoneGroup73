import { Component, OnInit, ChangeDetectorRef  } from '@angular/core';
import { Giaovien } from './shared/giaovien.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GiaovienService } from './shared/giaovien.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];
  addTeacherForm: FormGroup;
  editTeacherForm: FormGroup;
  selectedTeacher: any;

  constructor(private giaovienService: GiaovienService, private router: Router, private cdr: ChangeDetectorRef,private toastr: ToastrService,  private fb: FormBuilder) {
    this.addTeacherForm = this.fb.group({
      code: ['', Validators.required],
      ten: ['', [Validators.required, Validators.maxLength(30)]],
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]],
      truongDangDay: ['', Validators.required],
      province: ['', Validators.required],
      district: ['', Validators.required],
      diaChiCuThe: ['', [Validators.required, Validators.maxLength(100)]]
    });
    this.editTeacherForm = this.fb.group({
      code: [{ value: '', disabled: true }], // Code không được chỉnh sửa
      ten: ['', [Validators.required, Validators.maxLength(30)]],
      gioiTinh: ['Nam', Validators.required],
      ngaySinh: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      soDienThoai: ['', [Validators.required, Validators.pattern(/^0\d{9,10}$/)]],
      truongDangDay: ['', Validators.required],
      province: ['', Validators.required],
      district: ['', Validators.required],
      diaChiCuThe: ['', [Validators.required, Validators.maxLength(100)]],
      status: [true]
    });
    this.selectedTeacher = null;
  }

  ngOnInit(): void {
    this.loadDanhSachGiaoVien();
    this.loadProvinces();
  }

  loadProvinces() {
    this.giaovienService.getProvinces().subscribe(
        data => {
            console.log("Dữ liệu tỉnh/thành phố từ API:", data); // Debug xem API có trả về dữ liệu không
            this.provinces = data;
        },
        error => {
            console.error('Lỗi tải tỉnh/thành phố:', error);
        }
    );
}


// Khi chọn tỉnh/thành phố -> cập nhật quận/huyện
onProvinceChange(provinceCode: string) {
    console.log("Giá trị tỉnh/thành phố được chọn:", provinceCode); // Kiểm tra giá trị từ select dropdown

    const selectedProvince = this.provinces.find(p => p.code == provinceCode); // Kiểm tra kiểu dữ liệu

    if (selectedProvince) {
      console.log("Tỉnh đã chọn:", selectedProvince);
      console.log("Danh sách quận/huyện:", selectedProvince.districts);
      this.districts = selectedProvince.districts;
  } else {
      console.warn("Không tìm thấy tỉnh/thành phố trong danh sách!");
      this.districts = [];
  }
  this.addTeacherForm.patchValue({ district: '' }); 
    
}



// Khi chọn tỉnh/thành phố trong modal chỉnh sửa -> cập nhật quận/huyện
onProvinceChangeForEdit(provinceCode: string) {
    console.log("Giá trị tỉnh/thành phố được chọn trong Edit:", provinceCode);

    const selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));

    if (selectedProvince) {
        console.log("Tỉnh đã chọn trong Edit:", selectedProvince);
        console.log("Danh sách quận/huyện trong Edit:", selectedProvince.districts);
        this.editDistricts = selectedProvince.districts;
    } else {
        console.warn("Không tìm thấy tỉnh/thành phố trong danh sách Edit!");
        this.editDistricts = [];
    }

   
}
  
   /** 🏫 Load danh sách giáo viên từ API */
   loadDanhSachGiaoVien() {
    let isActiveFilter: boolean | null = this.trangThai === 'Hoạt động' ? true : this.trangThai === 'Tạm ngừng' ? false : null;

    this.giaovienService.getDanhSachGiaoVien(this.currentPage, this.pageSize, this.searchTerm, '', isActiveFilter)
      .subscribe(response => {
        if (!response.isError && response.data) {
          this.students = response.data.map((gv: any) => ({
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

          this.totalPages = Math.ceil(response.data.length / this.pageSize);
        }
        this.cdr.detectChanges();
      });
  }

  

  /** Chuyển trang */
  goToPage(page: number) {
    this.currentPage = page;
    this.loadDanhSachGiaoVien();
  }

  /** Tìm kiếm giáo viên */
  searchGiaoVien() {
    this.currentPage = 1;
    this.loadDanhSachGiaoVien();
  }

  /** Mở rộng chi tiết giáo viên */
  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  openEditTeacherModal(teacher: any) {
    console.log("🔍 Giáo viên được chọn để chỉnh sửa:", teacher);
  
    this.selectedTeacher = { ...teacher };
  
    if (!teacher.code || teacher.code.trim() === "") {
      console.error("❌ Lỗi: Giáo viên không có mã!");
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
      status: teacher.isActive ? true : false // 🆕 Gán trạng thái khi mở form
    });
  
    console.log("✅ Dữ liệu sau khi gán vào form:", this.editTeacherForm.value);
  
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
    this.addTeacherForm.reset();
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  /** 📩 Gửi API để thêm giáo viên */
  submitNewTeacher() {
    if (this.addTeacherForm.invalid) {
      this.addTeacherForm.markAllAsTouched();
      return;
    }
  
    const formData = this.addTeacherForm.value;
  
    console.log("🔍 Kiểm tra dữ liệu form:", formData);
  
    // 🔹 Kiểm tra province và district
    const provinceObj = this.provinces.find(p => p.code == formData.province);
    const provinceName = provinceObj ? provinceObj.name : '';
  
    const districtObj = this.districts.find(d => d.code == formData.district);
    const districtName = districtObj ? districtObj.name : '';
  
    console.log("✅ Province Name:", provinceName);
    console.log("✅ District Name:", districtName);
  
    // 🔹 Nếu không có giá trị thì gán là chuỗi rỗng để tránh lỗi `undefined`
    const diaChiFormatted = `${provinceName || ''}, ${districtName || ''}, ${formData.diaChiCuThe || ''}`.trim();
  
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
  
    console.log(" Gửi API thêm giáo viên:", newTeacher);
  
    this.giaovienService.createGiaoVien(newTeacher).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success("Thêm giáo viên thành công!", "Thành công");
          this.closeModal();
          this.loadDanhSachGiaoVien();
        } else {
          this.toastr.error(res.message, "Lỗi");
        }
      },
      error: () => {
        this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
      }
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
    this.districts = []; // Reset danh sách quận/huyện
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

    this.selectedTeacher = { ...teacher }; // 🆕 Lưu lại giáo viên đang chỉnh sửa

    console.log("🔍 Giáo viên được chọn:", this.selectedTeacher);

    // 🏢 Tách địa chỉ thành phần riêng (Tỉnh, Quận/Huyện, Địa chỉ cụ thể)
    const addressParts = teacher.diaChi ? teacher.diaChi.split(',').map(part => part.trim()) : ['', '', ''];
    const provinceName = addressParts[0] || ''; // Thành phố
    const districtName = addressParts[1] || ''; // Quận/Huyện
    const detailAddress = addressParts[2] || ''; // Địa chỉ cụ thể

    // 🔹 Lấy `provinceCode` từ `provinceName`
    const provinceObj = this.provinces.find(p => p.name === provinceName);
    const provinceCode = provinceObj ? provinceObj.code : '';

    // 🔹 Gọi danh sách quận/huyện theo tỉnh đã chọn
    this.onProvinceChangeForEdit(provinceCode);

    // 🔹 Lấy `districtCode` từ `districtName`
    const districtObj = provinceObj?.districts.find(d => d.name === districtName);
    const districtCode = districtObj ? districtObj.code : '';

    // 🗓️ Chuyển đổi ngày sinh về `yyyy-MM-dd`
    const ngaySinhFormatted = teacher.ngaySinh ? new Date(teacher.ngaySinh).toISOString().split('T')[0] : '';

    // 🔹 Cập nhật form với dữ liệu chỉnh sửa
    this.editTeacherForm.patchValue({
        code: teacher.code, // ✅ Đảm bảo code được gán đúng
        ten: teacher.ten,
        gioiTinh: teacher.gioiTinh,
        ngaySinh: ngaySinhFormatted,
        email: teacher.email,
        soDienThoai: teacher.soDienThoai,
        truongDangDay: teacher.truongDangDay,
        province: provinceCode,
        district: districtCode,
        diaChiCuThe: detailAddress
    });

    console.log("✅ Dữ liệu trong form sau khi gán:", this.editTeacherForm.value);
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
  console.log("🔍 Giáo viên đang chỉnh sửa:", this.selectedTeacher);
  console.log("🔍 Dữ liệu từ Form:", this.editTeacherForm.value);

  let formData = { ...this.editTeacherForm.value };

  // 🔥 Kiểm tra nếu `code` bị undefined thì lấy từ `selectedTeacher`
  if (!formData.code || formData.code.trim() === "") {
    if (this.selectedTeacher && this.selectedTeacher.code) {
      formData.code = this.selectedTeacher.code;
    } else {
      this.toastr.error("Không tìm thấy mã giáo viên!", "Lỗi");
      console.error("❌ Lỗi: Mã giáo viên không tồn tại!");
      return;
    }
  }

  console.log("✅ Mã giáo viên sau khi gán:", formData.code);

  // 🔹 Chuyển đổi ngày sinh sang định dạng `YYYY-MM-DD`
  if (formData.ngaySinh) {
    const date = new Date(formData.ngaySinh);
    formData.ngaySinh = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  }

  // 🔹 Định dạng lại `diaChi`
  const provinceObj = this.provinces.find(p => p.code == formData.province);
  const provinceName = provinceObj ? provinceObj.name : '';

  const districtObj = this.editDistricts.find(d => d.code == formData.district);
  const districtName = districtObj ? districtObj.name : '';

  formData.diaChi = `${provinceName}, ${districtName}, ${formData.diaChiCuThe || ''}`.trim();

  // 🔹 Chuyển đổi status thành string "true" hoặc "false"
  formData.status = formData.status ? "true" : "false";

  console.log("📤 Gửi API cập nhật giáo viên với dữ liệu:", JSON.stringify(formData));

  // 🛠 Gửi API cập nhật
  this.giaovienService.updateGiaoVien(formData).subscribe({
    next: (res) => {
      console.log("✅ Phản hồi từ API:", res);
      if (!res.isError) {
        this.toastr.success("Cập nhật giáo viên thành công!", "Thành công");
        this.closeEditModal();
        this.loadDanhSachGiaoVien();
      } else {
        this.toastr.error(res.message, "Lỗi");
      }
    },
    error: (error) => {
      console.error("❌ Lỗi kết nối API:", error);
      this.toastr.error("Có lỗi xảy ra, vui lòng thử lại!", "Lỗi");
    }
  });
}

  
  

  filterByStatus() {
    this.currentPage = 1;
    this.loadDanhSachGiaoVien();
  }
  /** Hàm format ngày => yyyy-MM-dd */
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
  
    onExportFile() {
      alert('Xuất Tệp');
    }
  

}

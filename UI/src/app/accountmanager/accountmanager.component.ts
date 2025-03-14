import { Component, OnInit } from '@angular/core';
import { AccountmanagerService } from './shared/account.service';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-accountmanager',
  templateUrl: './accountmanager.component.html',
  styleUrls: ['./accountmanager.component.scss']
})
export class AccountmanagerComponent implements OnInit {
  selectedProvince: any = null;
  selectedDistrict: any = null;
  trangThai: string = '';
  searchTerm: string = '';
  cosoList: any[] = [];
  students: any[] = [];

  currentPage: number = 1;
  totalPages: number = 2;

  isModalOpen: boolean = false;
  isEditModalOpen: boolean = false;

  newStudent: any = {
    code: '', ten: '', gioiTinh: 'Nam', ngaySinh: '', email: '',
    soDienThoai: '', coSoId: '', province: '', district: '', diaChi: ''
  };

  editStudent: any = {
    code: '', ten: '', gioiTinh: 'Nam', ngaySinh: '', email: '',
    soDienThoai: '', coSoId: '', province: '', district: '', diaChi: ''
  };

  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];

  constructor(private accountmanagerService: AccountmanagerService, private cd: ChangeDetectorRef) {}


  ngOnInit(): void {
    this.accountmanagerService.getProvinces().subscribe(
      data => {
        this.provinces = data;
      },
      error => console.error('Error fetching provinces:', error)
    );
    this.loadDanhSachNhanVien();
    this.loadDanhSachCoSo(); // Tải danh sách cơ sở khi khởi tạo component
    console.log(this.cosoList);
  }
  

  // Tải danh sách nhân viên
  loadDanhSachNhanVien() {
    this.accountmanagerService.getDanhSachNhanVien(1, 100, '').subscribe(
      response => {
        console.log('Dữ liệu nhân viên từ API:', response);
        if (response && response.data) {
          this.students = response.data.items.map(student => ({
            ...student,
             // Hiển thị luôn khi load trang
          }));
          this.cd.detectChanges();  // Buộc Angular cập nhật UI ngay
        }
      },
      error => console.error('Lỗi tải danh sách nhân viên:', error)
    );
  }
  
  

  // Tải danh sách cơ sở
  loadDanhSachCoSo() {
    this.accountmanagerService.getDanhSachCoSo(1, 100, '').subscribe(
      response => {
        console.log('Dữ liệu từ API:', response); 
        if (response && response.data) {
          this.cosoList = response.data.items; 
          console.log('Danh sách cơ sở:', this.cosoList);
        }
      },
      error => {
        console.error('Lỗi tải danh sách cơ sở:', error);
      }
    );
  }
  

  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  openAddStudentModal() { this.isModalOpen = true; }
  closeModal() { this.isModalOpen = false; }

  submitNewStudent() {
    
    const provinceName = this.getProvinceName();
    const districtName = this.getDistrictName();

    // Đảm bảo mã nhân viên luôn có tiền tố "NV"
    

    const fullAddress = `${this.newStudent.diaChi}, ${districtName}, ${provinceName}`;

    const newHs = {
        code: this.newStudent.code,
        ten: this.newStudent.ten,
        gioiTinh: this.newStudent.gioiTinh,
        ngaySinh: this.newStudent.ngaySinh ? this.formatDate(this.newStudent.ngaySinh) : '',
        email: this.newStudent.email,
        soDienThoai: this.newStudent.soDienThoai,
        coSoId: this.newStudent.coSoId || null,  
        diaChi: fullAddress.trim() !== ", ," ? fullAddress : "Không xác định",
        role: "CampusManager"
    };

    console.log("📌 Dữ liệu gửi lên API:", newHs);

    this.accountmanagerService.createNhanVien(newHs).subscribe(
        response => {
            console.log('✅ Thêm nhân viên thành công:', response);
            this.loadDanhSachNhanVien(); 
            this.isModalOpen = false;
        },
        error => {
            console.error('❌ Lỗi khi thêm nhân viên:', error);
            alert('Lỗi khi thêm nhân viên. Kiểm tra API hoặc dữ liệu đầu vào!');
        }
    );
}
isUnderage: boolean = false;
validateEmail(email: string): boolean {
  const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  return emailPattern.test(email);
}
validatePhoneNumber(phone: string): boolean {
  const phonePattern = /^0[0-9]{9}$/; // Bắt đầu bằng số 0 và có tổng cộng 10 chữ số
  return phonePattern.test(phone);
}
checkAge(ngaySinh: string) {
  if (!ngaySinh) {
    this.isUnderage = false;
    return;
  }

  const birthDate = new Date(ngaySinh);
  const today = new Date();
  const age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();
  const dayDiff = today.getDate() - birthDate.getDate();

  // Kiểm tra nếu chưa đủ 18 tuổi
  this.isUnderage = (age < 18 || (age === 18 && (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0))));
}


  
getProvinceName(): string {
  if (this.selectedProvince) {
      console.log("🔍 Tỉnh/Thành phố đã chọn:", this.selectedProvince.name);
      return this.selectedProvince.name;
  }
  return "Không xác định";
}

getDistrictName(): string {
  return this.selectedDistrict ? this.selectedDistrict.name : "Không xác định";
}
forceNVPrefix() {
  if (!this.newStudent.code.startsWith("NV-")) {
      this.newStudent.code = "NV-" + this.newStudent.code.replace(/^NV-/, ""); 
  }
}







onProvinceChange(provinceCode: string) {
  this.selectedProvince = this.provinces.find(p => p.code == provinceCode);
  this.selectedDistrict = null; // Reset district khi chọn tỉnh/thành phố mới

  if (this.selectedProvince) {
      this.districts = this.selectedProvince.districts || [];  
      console.log("🌍 Đã chọn tỉnh/thành phố:", this.selectedProvince.name);
      console.log("🏙️ Danh sách quận/huyện cập nhật:", this.districts);
  } else {
      this.districts = [];
      console.log("⚠️ Không tìm thấy tỉnh/thành phố!");
  }
}


onDistrictChange(districtCode: string) {
    console.log("📌 Mã quận/huyện nhận được khi thêm mới:", districtCode);

    if (!this.districts || this.districts.length === 0) {
        console.error("⚠️ Không có danh sách quận/huyện để tìm kiếm khi thêm mới!");
        return;
    }

    // Chuyển districtCode về cùng kiểu dữ liệu với danh sách
    const isDistrictCodeString = typeof this.districts[0].code === "string";
    const districtCodeFormatted = isDistrictCodeString ? String(districtCode) : Number(districtCode);

    const availableDistrictCodes = this.districts.map(d => isDistrictCodeString ? String(d.code) : Number(d.code));
    console.log("📌 Danh sách mã quận/huyện có sẵn khi thêm mới:", availableDistrictCodes);

    if (!availableDistrictCodes.includes(districtCodeFormatted)) {
        console.error(`⚠️ Không tìm thấy quận/huyện với mã: ${districtCodeFormatted}`);
        return;
    }

    this.selectedDistrict = this.districts.find(d => isDistrictCodeString ? String(d.code) === String(districtCodeFormatted) : Number(d.code) === Number(districtCodeFormatted));
    console.log("🏙️ Đã chọn quận/huyện khi thêm mới:", this.selectedDistrict?.name);
}





onEditStudentClick(index: number) {
  const hs = this.students[index];

  // Sao chép dữ liệu để chỉnh sửa
  this.editStudent = { 
      ...hs, 
      ngaySinh: this.formatDate(hs.ngaySinh) 
  };

  // Xác định tỉnh/thành phố
  this.selectedProvince = this.provinces.find(p => Number(p.code) === Number(this.editStudent.province));
  this.editDistricts = this.selectedProvince ? this.selectedProvince.districts || [] : [];

  // Xác định quận/huyện
  const districtCode = Number(this.editStudent.district);
  this.selectedDistrict = this.editDistricts.find(d => Number(d.code) === districtCode) || null;

  // Loại bỏ tỉnh/thành phố và quận/huyện ra khỏi địa chỉ cụ thể
  if (this.editStudent.diaChi) {
      const parts = this.editStudent.diaChi.split(',').map(p => p.trim());
      const addressParts = parts.slice(0, parts.length - 2); // Loại bỏ 2 phần cuối (tỉnh + quận/huyện)
      this.editStudent.diaChi = addressParts.join(', '); // Ghép lại thành địa chỉ cụ thể
  }

  console.log("🏙️ Quận/Huyện đang sửa:", this.selectedDistrict ? this.selectedDistrict.name : "Không xác định");
  console.log("📌 Địa chỉ cụ thể khi sửa:", this.editStudent.diaChi);

  this.isEditModalOpen = true;
}




  closeEditModal() { this.isEditModalOpen = false; }

  submitEditStudent() {
    
    const provinceName = this.getProvinceName();
    const districtName = this.getDistrictName();

    // Chỉ lấy địa chỉ cụ thể mà không bao gồm tỉnh/thành phố và quận/huyện
    const fullAddress = `${this.editStudent.diaChi.trim()}, ${districtName}, ${provinceName}`;

    // Chuẩn bị dữ liệu gửi API
    const updatedHs = {
        code: this.editStudent.code,
        ten: this.editStudent.ten,
        gioiTinh: this.editStudent.gioiTinh,
        diaChi: fullAddress.trim() !== ", ," ? fullAddress : "Không xác định",
        ngaySinh: this.editStudent.ngaySinh ? this.formatDate(this.editStudent.ngaySinh) : '',
        email: this.editStudent.email,
        soDienThoai: this.editStudent.soDienThoai,
        coSoId: this.editStudent.coSoId || null, 
        userId: this.editStudent.userId || null,
        role: "CampusManager"
    };

    console.log("📌 Dữ liệu gửi lên API khi sửa:", updatedHs);

    this.accountmanagerService.updateNhanVien(updatedHs).subscribe(
        response => {
            console.log('✅ Cập nhật nhân viên thành công:', response);
            this.loadDanhSachNhanVien(); 
            this.isEditModalOpen = false;
        },
        error => {
            console.error('❌ Lỗi khi cập nhật nhân viên:', error);
            alert('Lỗi khi cập nhật nhân viên. Kiểm tra API hoặc dữ liệu đầu vào!');
        }
    );
}





onProvinceChangeForEdit(provinceCode: string) {
  console.log("📌 Mã tỉnh/thành phố nhận được khi chỉnh sửa:", provinceCode);
  
  // Tìm tỉnh/thành phố theo mã
  this.selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));

  if (this.selectedProvince) {
      this.editDistricts = this.selectedProvince.districts || [];
      console.log("🏙️ Danh sách quận/huyện mới khi chỉnh sửa:", this.editDistricts);
  } else {
      this.editDistricts = [];
      console.log("⚠️ Không tìm thấy tỉnh/thành phố khi chỉnh sửa!");
  }

  // Reset quận/huyện khi chọn tỉnh mới
  this.selectedDistrict = null;
}

onDistrictChangeForEdit(districtCode: string) {
  console.log("📌 Mã quận/huyện nhận được khi chỉnh sửa:", districtCode);

  if (!this.editDistricts || this.editDistricts.length === 0) {
      console.error("⚠️ Không có danh sách quận/huyện để tìm kiếm khi chỉnh sửa!");
      return;
  }

  // Đảm bảo kiểu dữ liệu quận/huyện khớp với danh sách
  const isDistrictCodeString = typeof this.editDistricts[0].code === "string";
  const districtCodeFormatted = isDistrictCodeString ? String(districtCode).trim() : Number(districtCode.trim());

  console.log("📌 Danh sách mã quận/huyện có sẵn khi chỉnh sửa:", 
              this.editDistricts.map(d => d.code));

  // Tìm quận/huyện tương ứng
  this.selectedDistrict = this.editDistricts.find(d => 
      isDistrictCodeString ? String(d.code) === String(districtCodeFormatted) 
                           : Number(d.code) === Number(districtCodeFormatted)
  );

  if (this.selectedDistrict) {
      console.log("🏙️ Đã chọn quận/huyện khi chỉnh sửa:", this.selectedDistrict.name);
  } else {
      console.error(`⚠️ Không tìm thấy quận/huyện với mã: ${districtCodeFormatted}`);
  }
}








  formatDate(date: any) {
    if (!date) return '';
    const d = new Date(date);
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
  }
  
}

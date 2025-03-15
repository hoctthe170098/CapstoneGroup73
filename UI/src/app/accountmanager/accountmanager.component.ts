import { Component, OnInit } from '@angular/core';
import { AccountmanagerService } from './shared/account.service';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

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
    code: '', ten: '', gioiTinh: '', ngaySinh: '', email: '',
    soDienThoai: '', coSoId: '', province: '', district: '', diaChi: '',role:''
  };

  editStudent: any = {
    code: '', ten: '', gioiTinh: '', ngaySinh: '', email: '',
    soDienThoai: '', coSoId: '', province: '', district: '', diaChi: '',role:''
  };

  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];
  isUnderage: boolean = false;
  isCodeEmpty: boolean = false;
  constructor(private accountmanagerService: AccountmanagerService, private cd: ChangeDetectorRef,private toastr: ToastrService) {}


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
    this.accountmanagerService.getDanhSachCoSo().subscribe(
      response => {
        console.log('Dữ liệu từ API:', response); 
        if (response && response.data) {
          this.cosoList = response.data; 
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
    this.forceNVPrefix();
    
    if (!this.newStudent.code || this.newStudent.code.trim() === '') {
     ;
      return;
    }
    if (this.newStudent.code.length > 18) {
      
      return;
    }
    if (!this.newStudent.ten || this.newStudent.ten.trim() === '') {
      
      return;
    }
    if (this.newStudent.ten.length > 30) {
      
      return;
    }
    if (!this.validatePhoneNumber(this.newStudent.soDienThoai)) {
      
      return;
      
    }
    
    
    
    const provinceName = this.getProvinceName();
    const districtName = this.getDistrictName();

    // Đảm bảo mã nhân viên luôn có tiền tố "NV"
    

    const fullAddress = `${this.newStudent.diaChi}, ${districtName}, ${provinceName}`;

    const newHs = {
      code: this.newStudent.code && this.newStudent.code.startsWith("NV") 
      ? this.newStudent.code.replace(/^NV/, "") 
      : this.newStudent.code,
        ten: this.newStudent.ten,
        gioiTinh: this.newStudent.gioiTinh,
        ngaySinh: this.newStudent.ngaySinh ? this.formatDate(this.newStudent.ngaySinh) : '',
        email: this.newStudent.email,
        soDienThoai: this.newStudent.soDienThoai,
        coSoId: this.newStudent.coSoId || null,  
        diaChi: fullAddress.trim() !== ", ," ? fullAddress : "Không xác định",
        role: this.newStudent.role
    };

    console.log("📌 Dữ liệu gửi lên API:", newHs);

    this.accountmanagerService.createNhanVien(newHs).subscribe(
      response => {
        if (!response.isError) {
          this.toastr.success(response.message);
          this.loadDanhSachNhanVien();
          this.isModalOpen = false;
        } else {
          // ✅ Nếu có lỗi từ API, hiển thị thông báo từ `errors`
          if (response.errors && response.errors.length > 0) {
            this.toastr.error(response.errors.join("\n")); // Hiển thị tất cả lỗi từ API
          } else {
            this.toastr.error(response.message);
          }
          this.isModalOpen = true;
        }
      },
        error => {
          console.error('❌ Lỗi khi thêm nhân viên:', error);
          if (error.error && error.error.isError) {
            // ✅ Kiểm tra lỗi từ API và hiển thị
            if (error.error.errors && error.error.errors.length > 0) {
              alert(`🚨 ${error.error.errors.join("\n")}`);
            } else {
              alert(`⚠️ ${error.error.message || 'Lỗi không xác định'}`);
            }
          } else {
            alert('⚠️ Lỗi khi thêm nhân viên. Kiểm tra API hoặc dữ liệu đầu vào!');
          }
        }
    );
}

validateEmail(email: string): boolean {
  const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  return emailPattern.test(email);
}
validatePhoneNumber(phone: string): boolean {
  if (!phone) return false; 
  phone = phone.trim();  // Loại bỏ khoảng trắng thừa
  const phonePattern = /^0\d{9,10}$/; // Chấp nhận 10-11 số và bắt đầu bằng 0
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
  if (!this.newStudent.code.startsWith('NV')) {
    this.newStudent.code = 'NV' + this.newStudent.code.replace(/^NV/, '').trim();
  }
  if (this.newStudent.code.length > 18) {
    this.newStudent.code = this.newStudent.code.substring(0, 18);
  }
}

forceNVPrefixEdit() {
  if (!this.editStudent.code.startsWith('NV')) {
    this.editStudent.code = 'NV' + this.editStudent.code.replace(/^NV/, '').trim();
  }
  if (this.editStudent.code.length > 18) {
    this.editStudent.code = this.editStudent.code.substring(0, 18);
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
    ngaySinh: this.formatDate(hs.ngaySinh),
   
};
console.log("📌 Vai trò của nhân viên trước khi sửa:", this.editStudent.role)
  // Tìm cosoId từ cosoList dựa trên hs.tenCoSo
  console.log('Dữ liệu nhân viên:', hs);
  console.log('Các key của hs:', Object.keys(hs));

  console.log('Giá trị role nhận từ API:', hs.role);
  const coSo = this.cosoList.find(cs => cs.ten === hs.tenCoSo);
  this.editStudent.coSoId = coSo ? coSo.id : null;
  if (!this.editStudent.gioiTinh) {
    this.editStudent.gioiTinh = "Nam"; 
}
const validRoles = ["CampusManager", "LearningManager"];
  if (!validRoles.includes(this.editStudent.role)) {
    // Nếu API trả về giá trị không hợp lệ (hoặc rỗng) thì không gán mặc định để tránh ép cứng
    // Bạn có thể giữ giá trị rỗng để người dùng tự chọn lại
    this.editStudent.role = "";
  }


  if (this.editStudent.diaChi) {
      const parts = this.editStudent.diaChi.split(',').map(p => p.trim());

      if (parts.length >= 3) {  
          const provinceName = parts[parts.length - 1];  
          const districtName = parts[parts.length - 2];  
          const specificAddress = parts.slice(0, parts.length - 2).join(', ');  

          // Tìm **Tỉnh/Thành phố**
          this.selectedProvince = this.provinces.find(p => p.name === provinceName);
          if (this.selectedProvince) {
              this.editStudent.province = this.selectedProvince.code;
              this.editDistricts = this.selectedProvince.districts || [];

              // 🟢 Tìm **Quận/Huyện**
              this.selectedDistrict = this.editDistricts.find(d => d.name === districtName);
              if (this.selectedDistrict) {
                  this.editStudent.district = this.selectedDistrict.code;
              }
          }
          this.editStudent.diaChi = specificAddress;
      }
  }
  this.isEditModalOpen = true;
}







  closeEditModal() { this.isEditModalOpen = false; }

  submitEditStudent() {
    this.forceNVPrefixEdit(); // Đảm bảo Code đúng định dạng khi chỉnh sửa

    if (!this.editStudent.soDienThoai || this.editStudent.soDienThoai.trim() === '') {
     
      return;
    }
    if (!this.validatePhoneNumber(this.editStudent.soDienThoai)) {
     
      return;
    }
  
  if (!this.editStudent.ten || this.editStudent.ten.trim() === '') {
   
    return;
  }
  if (this.editStudent.ten.length > 30) {
   
    return;
  }
    const provinceName = this.selectedProvince ? this.selectedProvince.name : "Không xác định";
    const districtName = this.selectedDistrict ? this.selectedDistrict.name : "Không xác định";

    const fullAddress = `${this.editStudent.diaChi.trim()}, ${districtName}, ${provinceName}`;

    const updatedHs = {
        code: this.editStudent.code,
        ten: this.editStudent.ten,
        gioiTinh: this.editStudent.gioiTinh,
        diaChi: fullAddress.trim() !== ", Không xác định, Không xác định" ? fullAddress : "Không xác định",
        ngaySinh: this.editStudent.ngaySinh ? this.formatDate(this.editStudent.ngaySinh) : '',
        email: this.editStudent.email,
        soDienThoai: this.editStudent.soDienThoai,
        coSoId: this.editStudent.coSoId || null, 
        role: this.editStudent.role,
        status: "true"
    };

    console.log("📌 Dữ liệu cập nhật gửi API:", updatedHs);

    this.accountmanagerService.updateNhanVien(updatedHs).subscribe(
        response => {
            if (!response.isError) {
                this.toastr.success(response.message);
                this.isEditModalOpen = false;
            } else {
                this.toastr.error(response.message);
                this.isEditModalOpen = true;
            }
            this.loadDanhSachNhanVien();          
        },
        error => {
            alert('Lỗi khi cập nhật nhân viên. Kiểm tra API hoặc dữ liệu đầu vào!');
        }
    );
}






onProvinceChangeForEdit(provinceCode: string) {
  this.selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));

  if (this.selectedProvince) {
      this.editDistricts = this.selectedProvince.districts || [];
      this.editStudent.district = '';  // Reset quận/huyện khi đổi tỉnh
  } else {
      this.editDistricts = [];
  }
}


onDistrictChangeForEdit(districtCode: string) {
  if (!this.editDistricts || this.editDistricts.length === 0) {
    console.error("⚠️ Không có danh sách quận/huyện để tìm kiếm!");
    return;
  }

  console.log("📌 Mã quận/huyện nhận được khi chỉnh sửa:", districtCode);
  
  // Xác định kiểu dữ liệu của mã quận/huyện
  const isDistrictCodeString = typeof this.editDistricts[0].code === "string";
  const districtCodeFormatted = isDistrictCodeString ? String(districtCode).trim() : Number(districtCode.trim());

  // Tìm quận/huyện tương ứng
  this.selectedDistrict = this.editDistricts.find(d => 
    isDistrictCodeString ? String(d.code) === String(districtCodeFormatted) 
                         : Number(d.code) === Number(districtCodeFormatted)
  );

  if (this.selectedDistrict) {
    this.editStudent.district = this.selectedDistrict.code;
    console.log(`✅ Đã chọn quận/huyện: ${this.selectedDistrict.name} (${this.selectedDistrict.code})`);
  } else {
    console.error(`⚠️ Không tìm thấy quận/huyện có mã: ${districtCodeFormatted}`);
    this.editStudent.district = '';  // Reset giá trị nếu không tìm thấy
  }
}










  formatDate(date: any) {
    if (!date) return '';
    const d = new Date(date);
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
  }
  
}

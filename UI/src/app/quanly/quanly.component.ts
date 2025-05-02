import { Component, OnInit } from '@angular/core';
import { AccountmanagerService } from './shared/quanly.service';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-accountmanager',
  templateUrl: './quanly.component.html',
  styleUrls: ['./quanly.component.scss']
})
export class AccountmanagerComponent implements OnInit {
  selectedProvince: any = null;
  selectedDistrict: any = null;
  trangThai: string = '';
  searchTerm: string = '';
  cosoId: string = '';
  roleName: string = '';
  cosoList: any[] = [];
  managers: any[] = [];

  currentPage: number = 1;
  totalPages: number = 1;
  totalItems: number = 0; // Tổng số bản ghi
  pageSize: number = 8;

  isModalOpen: boolean = false;
  isEditModalOpen: boolean = false;

  newmanager: any = {
    code: '', ten: '', gioiTinh: '', ngaySinh: '', email: '',
    soDienThoai: '', coSoId: '', province: '', district: '', diaChi: '', role: '', status: 'true'
  };

  editmanager: any = {
    code: '', ten: '', gioiTinh: '', ngaySinh: '', email: '',
    soDienThoai: '', coSoId: '', province: '', district: '', diaChi: '', role: '', status: ''
  };

  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];
  isUnderage: boolean = false;
  isCodeEmpty: boolean = false;
  isRoleInvalid: boolean = false;
  isCoSoInvalid: boolean = false;

  constructor(private accountmanagerService: AccountmanagerService, 
    private cd: ChangeDetectorRef,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router
) { }


  ngOnInit(): void {
    this.accountmanagerService.getProvinces().subscribe(
      data => {
        this.provinces = data;
      },
      error => console.error('Error fetching provinces:', error)
    );
    this.loadDanhSachNhanVien();
    this.loadDanhSachCoSo(); // Tải danh sách cơ sở khi khởi tạo component
  }

  // Tải danh sách nhân viên
  loadDanhSachNhanVien() {
    // Chuyển dropdown "trangThai" sang giá trị isActive (true, false, hoặc null nếu không chọn)
    let isActiveFilter: boolean | null = null;
    if (this.trangThai === 'Hoạt động') {
      isActiveFilter = true;
    } else if (this.trangThai === 'Tạm ngừng') {
      isActiveFilter = false;
    }
    this.spinner.show();
    // Gọi API với payload bao gồm isActive
    this.accountmanagerService.getDanhSachNhanVien(
      this.currentPage,  // trang hiện tại
      this.pageSize,     // số bản ghi mỗi trang
      this.searchTerm,
      this.cosoId,
      this.roleName,   // từ khóa tìm kiếm
      '',                // sortBy (nếu cần)
      isActiveFilter     // filter trạng thái theo isActive (boolean)
    ).subscribe(
      (response: any) => {
        this.spinner.hide();
        if (!response.isError && response.data) {
          // Map dữ liệu từ API, sử dụng trường isActive của API và tạo thuộc tính hiển thị trạng thái
          this.managers = response.data.items.map((manager: any) => ({
            ...manager,
            showDetails: false,
            isActive: manager.trangThai, // sử dụng isActive từ API (boolean)
            displayStatus: manager.trangThai ? 'Hoạt động' : 'Tạm ngừng'
          }));

          this.totalItems = response.data.totalCount || 0;
          this.totalPages = Math.ceil(this.totalItems / this.pageSize);
        } else {
          console.error("Lỗi khi tải danh sách nhân viên", response?.message || 'Không có response.message');
        }
        this.cd.detectChanges();
      },
      error => {
        this.spinner.hide();
        console.error("Lỗi kết nối API", error);
      }
    );
  }

  /** 🟢 Chuyển trang */
  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadDanhSachNhanVien();
    }
  }




  // Tải danh sách cơ sở
  loadDanhSachCoSo() {
    this.accountmanagerService.getDanhSachCoSo().subscribe(
      response => {
        if (response.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        if (response && response.data) {
          this.cosoList = response.data;
        }
      },
      error => {
        console.error('Lỗi tải danh sách cơ sở:', error);
      }
    );
  }
  filterByCoSo() {
    this.currentPage = 1;
    this.loadDanhSachNhanVien();
  }
  filterByRole() {
    this.currentPage = 1;
    this.loadDanhSachNhanVien();
  }
  toggleDetails(index: number) {
    this.managers[index].showDetails = !this.managers[index].showDetails;
  }

  openAddManagerModal() { this.isModalOpen = true; }
  closeModal() { this.isModalOpen = false; }

  submitNewmanager() {
   
    if (!this.newmanager.code || this.newmanager.code.trim() === '') {
      ;
      return;
    }
    if (this.newmanager.code.length > 18) {

      return;
    }
    if (!this.newmanager.ten || this.newmanager.ten.trim() === '') {

      return;
    }
    if (this.newmanager.ten.length > 30) {

      return;
    }
    if (!this.validatePhoneNumber(this.newmanager.soDienThoai)) {

      return;

    }




    const provinceName = this.getProvinceName();
    const districtName = this.getDistrictName();

   


    const fullAddress = `${this.newmanager.diaChi}, ${districtName}, ${provinceName}`;

    const newHs = {
      code: `${this.newmanager.code.trim()}`, // ✨ Luôn tự gắn NV vào trước
      ten: this.newmanager.ten,
      gioiTinh: this.newmanager.gioiTinh,
      ngaySinh: this.newmanager.ngaySinh ? this.formatDate(this.newmanager.ngaySinh) : '',
      email: this.newmanager.email,
      soDienThoai: this.newmanager.soDienThoai,
      coSoId: this.newmanager.coSoId || null,
      diaChi: fullAddress.trim() !== ", ," ? fullAddress : "Không xác định",
      role: this.newmanager.role
    };

    this.spinner.show();
    this.accountmanagerService.createNhanVien(newHs).subscribe(
      response => {
        this.spinner.hide();
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
        this.spinner.hide();
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
  validateCoSo() {
    if (!this.newmanager.coSoId || this.newmanager.coSoId.trim() === '') {
      this.isCoSoInvalid = true;
    } else {
      this.isCoSoInvalid = false;
    }
  }
  validateRole() {
    if (!this.newmanager.role || this.newmanager.role.trim() === '') {
      this.isRoleInvalid = true;
    } else {
      this.isRoleInvalid = false;
    }
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
  tenVaiTro(vaiTro: string) {
    if (vaiTro == "CampusManager") return "Quản lý cơ sở";
    return "Quản lý tài liệu học tập";
  }




  getProvinceName(): string {
    if (this.selectedProvince) {
      return this.selectedProvince.name;
    }
    return "Không xác định";
  }

  getDistrictName(): string {
    return this.selectedDistrict ? this.selectedDistrict.name : "Không xác định";
  }
  

  


  onProvinceChange(provinceCode: string) {
    this.selectedProvince = this.provinces.find(p => p.code == provinceCode);
    this.selectedDistrict = null; // Reset district khi chọn tỉnh/thành phố mới

    if (this.selectedProvince) {
      this.districts = this.selectedProvince.districts || [];
    } else {
      this.districts = [];
    }
  }


  onDistrictChange(districtCode: string) {

    if (!this.districts || this.districts.length === 0) {
      console.error("⚠️ Không có danh sách quận/huyện để tìm kiếm khi thêm mới!");
      return;
    }

    // Chuyển districtCode về cùng kiểu dữ liệu với danh sách
    const isDistrictCodeString = typeof this.districts[0].code === "string";
    const districtCodeFormatted = isDistrictCodeString ? String(districtCode) : Number(districtCode);

    const availableDistrictCodes = this.districts.map(d => isDistrictCodeString ? String(d.code) : Number(d.code));

    if (!availableDistrictCodes.includes(districtCodeFormatted)) {
      console.error(`⚠️ Không tìm thấy quận/huyện với mã: ${districtCodeFormatted}`);
      return;
    }

    this.selectedDistrict = this.districts.find(d => isDistrictCodeString ? String(d.code) === String(districtCodeFormatted) : Number(d.code) === Number(districtCodeFormatted));
  }





  onEditManagerClick(index: number) {
    const hs = this.managers[index];

    // Sao chép dữ liệu để chỉnh sửa
    this.editmanager = {
      ...hs,
      ngaySinh: this.formatDate(hs.ngaySinh),
      status: hs.isActive ? "true" : "false"

    };

    const coSo = this.cosoList.find(cs => cs.ten === hs.tenCoSo);
    this.editmanager.coSoId = coSo ? coSo.id : null;
    if (!this.editmanager.gioiTinh) {
      this.editmanager.gioiTinh = "Nam";
    }

    const validRoles = ["CampusManager", "LearningManager"];
    if (!validRoles.includes(this.editmanager.role)) {
      // Nếu API trả về giá trị không hợp lệ (hoặc rỗng) thì không gán mặc định để tránh ép cứng
      // Bạn có thể giữ giá trị rỗng để người dùng tự chọn lại
      this.editmanager.role = "";
    }


    if (this.editmanager.diaChi) {
      const parts = this.editmanager.diaChi.split(',').map(p => p.trim());

      if (parts.length >= 3) {
        const provinceName = parts[parts.length - 1];
        const districtName = parts[parts.length - 2];
        const specificAddress = parts.slice(0, parts.length - 2).join(', ');

        // Tìm **Tỉnh/Thành phố**
        this.selectedProvince = this.provinces.find(p => p.name === provinceName);
        if (this.selectedProvince) {
          this.editmanager.province = this.selectedProvince.code;
          this.editDistricts = this.selectedProvince.districts || [];

          // 🟢 Tìm **Quận/Huyện**
          this.selectedDistrict = this.editDistricts.find(d => d.name === districtName);
          if (this.selectedDistrict) {
            this.editmanager.district = this.selectedDistrict.code;
          }
        }
        this.editmanager.diaChi = specificAddress;
      }
    }
    this.isEditModalOpen = true;
  }
  closeEditModal() { this.isEditModalOpen = false; }
  submitEditmanager() {
   

    if (!this.editmanager.soDienThoai || this.editmanager.soDienThoai.trim() === '') {

      return;
    }
    if (!this.validatePhoneNumber(this.editmanager.soDienThoai)) {

      return;
    }

    if (!this.editmanager.ten || this.editmanager.ten.trim() === '') {

      return;
    }
    if (this.editmanager.ten.length > 30) {

      return;
    }

    const provinceName = this.selectedProvince ? this.selectedProvince.name : "Không xác định";
    const districtName = this.selectedDistrict ? this.selectedDistrict.name : "Không xác định";

    const fullAddress = `${provinceName}, ${districtName}, ${this.editmanager.diaChi.trim()}`;

    const updatedHs = {
      code: this.editmanager.code,
      ten: this.editmanager.ten,
      gioiTinh: this.editmanager.gioiTinh,
      diaChi: fullAddress.trim() !== ", Không xác định, Không xác định" ? fullAddress : "Không xác định",
      ngaySinh: this.editmanager.ngaySinh ? this.formatDate(this.editmanager.ngaySinh) : '',
      email: this.editmanager.email,
      soDienThoai: this.editmanager.soDienThoai,
      coSoId: this.editmanager.coSoId || null,
      role: this.editmanager.tenVaiTro,
      status: this.editmanager.status
    };

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
  searchNhanVien() {
    this.currentPage = 1;
    this.loadDanhSachNhanVien();
  }
  filterByStatus() {
    this.currentPage = 1;
    this.loadDanhSachNhanVien();
  }
  onProvinceChangeForEdit(provinceCode: string) {
    this.selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));

    if (this.selectedProvince) {
      this.editDistricts = this.selectedProvince.districts || [];
      this.editmanager.district = '';  // Reset quận/huyện khi đổi tỉnh
    } else {
      this.editDistricts = [];
    }
  }
  onDistrictChangeForEdit(districtCode: string) {
    if (!this.editDistricts || this.editDistricts.length === 0) {
      console.error("⚠️ Không có danh sách quận/huyện để tìm kiếm!");
      return;
    }


    // Xác định kiểu dữ liệu của mã quận/huyện
    const isDistrictCodeString = typeof this.editDistricts[0].code === "string";
    const districtCodeFormatted = isDistrictCodeString ? String(districtCode).trim() : Number(districtCode.trim());

    // Tìm quận/huyện tương ứng
    this.selectedDistrict = this.editDistricts.find(d =>
      isDistrictCodeString ? String(d.code) === String(districtCodeFormatted)
        : Number(d.code) === Number(districtCodeFormatted)
    );

    if (this.selectedDistrict) {
      this.editmanager.district = this.selectedDistrict.code;
    } else {
      console.error(`⚠️ Không tìm thấy quận/huyện có mã: ${districtCodeFormatted}`);
      this.editmanager.district = '';  // Reset giá trị nếu không tìm thấy
    }
  }
  formatDate(date: any) {
    if (!date) return '';
    const d = new Date(date);
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
  }

}

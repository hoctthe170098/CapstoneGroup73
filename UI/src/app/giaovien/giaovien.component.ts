import { Component, OnInit } from '@angular/core';
import { Giaovien } from './shared/giaovien.model';
import { GiaovienService } from './shared/giaovien.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-giaovien',
  templateUrl: './giaovien.component.html',
  styleUrls: ['./giaovien.component.scss']
})
export class GiaovienComponent implements OnInit {

   trangThai: string = '';
    lop: string = '';
    searchTerm: string = '';
    cosoList: any[] = [];

    students: (Giaovien & { showDetails: boolean })[] = [
      {
        code: 'HE171450',
        ten: 'Bùi Ngọc Dũng',
        gioiTinh: 'Nam',
        diaChi: '245 Phạm Ngọc Thạch, Đống Đa, TP Hà Nội',
        lop: 'Lớp 1',
        truongDangDay: 'Trường Đại học FPT',
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
        lopHocs: ['Toán 1', 'Tiếng Anh 1', 'Tiếng Việt 1'],
        showDetails: false
      },
      {
        code: 'HE171466',
        ten: 'Ngô Minh Kiên',
        gioiTinh: 'Nam',
        diaChi: 'Long Biên, Hà Nội',
        lop: 'Lớp 1',
        truongDangDay: 'Trường Đại học FPT',
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
        lopHocs: ['Lớp Toán 2', 'Lớp Anh 2'],
        showDetails: false
      }
    ];
  
    toggleDetails(index: number) {
      this.students[index].showDetails = !this.students[index].showDetails;
    }
  
    
  
    onChooseFileclick() {
      this.router.navigate(['/giaovien/import-giao-vien']);
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
      truongDangDay: '',
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
      console.log('Thêm giáo viên mới:', this.newStudent);
      const newHs: Giaovien & { showDetails: boolean } = {
        code: this.newStudent.code,
        ten: this.newStudent.ten,
        gioiTinh: this.newStudent.gioiTinh,
        ngaySinh: this.newStudent.ngaySinh ? new Date(this.newStudent.ngaySinh) : new Date(),
        email: this.newStudent.email,
        soDienThoai: this.newStudent.soDienThoai,
        diaChi: this.newStudent.diaChi,
        lop: '',
        truongDangDay: this.newStudent.truongDangDay,
        coSoId: '',
        coso: { id: '', ten: '', diaChi: '', soDienThoai: '', trangThai: '', default: false },
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
        truongDangDay: '',
        province: '',
        district: '',
        diaChi: ''
      };
      this.isModalOpen = false;
    }
  
    
    provinces: any[] = [];
    districts: any[] = [];
  
    constructor(private giaovienService: GiaovienService,private router: Router) {}
  
    ngOnInit(): void {
      this.giaovienService.getProvinces().subscribe(
        data => {
          this.provinces = data;
        },
        error => {
          console.error('Error fetching provinces:', error);
        }
      );
      this.loadDanhSachCoSo();
      console.log(this.cosoList);
    }
    loadDanhSachCoSo() {
      this.giaovienService.getDanhSachCoSo(1, 100, '').subscribe(
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
      truongDangDay: '',
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
        truongDangDay: hs.truongDangDay,
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
        updatedHs.truongDangDay = this.editStudent.truongDangDay;
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

import { Component, OnInit } from '@angular/core';
import { Accountmanager } from './shared/accountmanager.model';
import { AccountmanagerService } from './shared/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-accountmanager',
  templateUrl: './accountmanager.component.html',
  styleUrls: ['./accountmanager.component.scss']
})
export class AccountmanagerComponent implements OnInit {
  trangThai: string = '';
  lop: string = '';
  searchTerm: string = '';
  cosoList: any[] = [];
  
  students: (Accountmanager & { showDetails: boolean, role: string })[] = [
    {
      code: 'HE171450',
      ten: 'Bùi Ngọc Dũng',
      gioiTinh: 'Nam',
      diaChi: '245 Phạm Ngọc Thạch, Đống Đa, TP Hà Nội',
      lop: 'Lớp 1',
      ngaySinh: new Date(2003, 6, 23),
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
      role: 'Sinh viên',
      showDetails: false
    },
    {
      code: 'HE171466',
      ten: 'Ngô Minh Kiên',
      gioiTinh: 'Nam',
      diaChi: 'Long Biên, Hà Nội',
      lop: 'Lớp 1',
      ngaySinh: new Date(2003, 4, 10),
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
      role: 'Sinh viên',
      showDetails: false
    }
  ];

  currentPage: number = 1;
  totalPages: number = 2;

  goToPage(page: number) {
    this.currentPage = page;
  }

  isModalOpen: boolean = false;
  isEditModalOpen: boolean = false;

  newStudent: any = {
    code: '', ten: '', gioiTinh: 'Nam', ngaySinh: '', email: '',
    soDienThoai: '', truongDangDay: '', province: '', district: '', diaChi: ''
  };

  editStudent: any = {
    code: '', ten: '', gioiTinh: 'Nam', ngaySinh: '', email: '',
    soDienThoai: '', truongDangDay: '', province: '', district: '', diaChi: ''
  };

  provinces: any[] = [];
  districts: any[] = [];
  editDistricts: any[] = [];

  constructor(private giaovienService: AccountmanagerService, private router: Router) {}

  ngOnInit(): void {
    this.giaovienService.getProvinces().subscribe(
      data => this.provinces = data,
      error => console.error('Error fetching provinces:', error)
    );
    this.loadDanhSachCoSo();
  }

  loadDanhSachCoSo() {
    this.giaovienService.getDanhSachCoSo(1, 100, '').subscribe(
      response => {
        if (response && response.data) {
          this.cosoList = response.data.items;
        }
      },
      error => console.error('Lỗi tải danh sách cơ sở:', error)
    );
  }
  
  
  

  toggleDetails(index: number) {
    this.students[index].showDetails = !this.students[index].showDetails;
  }

  openAddStudentModal() { this.isModalOpen = true; }
  closeModal() { this.isModalOpen = false; }

  submitNewStudent() {
    const newHs: Accountmanager & { showDetails: boolean, role: string } = {
      ...this.newStudent,
      ngaySinh: this.newStudent.ngaySinh ? new Date(this.newStudent.ngaySinh) : new Date(),
      coSoId: '', coso: { id: '', ten: '', diaChi: '', soDienThoai: '', trangThai: '', default: false },
      lopHocs: [],
      showDetails: false,
      role: 'Sinh viên'
    };
    this.students.push(newHs);
    this.newStudent = { code: '', ten: '', gioiTinh: 'Nam', ngaySinh: '', email: '',
      soDienThoai: '', truongDangDay: '', province: '', district: '', diaChi: '' };
    this.isModalOpen = false;
  }

  onProvinceChange(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code == provinceCode);
    this.districts = selectedProvince ? selectedProvince.districts : [];
  }

  onEditStudentClick(index: number) {
    const hs = this.students[index];
    this.editStudent = { ...hs, ngaySinh: this.formatDate(hs.ngaySinh) };
    this.onProvinceChangeForEdit(this.editStudent.province);
    this.isEditModalOpen = true;
  }

  closeEditModal() { this.isEditModalOpen = false; }

  submitEditStudent() {
    const idx = this.students.findIndex(h => h.code === this.editStudent.code);
    if (idx !== -1) {
      this.students[idx] = { ...this.students[idx], ...this.editStudent, ngaySinh: new Date(this.editStudent.ngaySinh) };
    }
    this.isEditModalOpen = false;
  }

  onProvinceChangeForEdit(provinceCode: string) {
    const selectedProvince = this.provinces.find(p => p.code === provinceCode);
    this.editDistricts = selectedProvince ? selectedProvince.districts : [];
  }

  formatDate(date: Date) {
    return date ? `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}` : '';
  }
  
}
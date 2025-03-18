import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';


@Component({
  selector: 'app-edit-lophoc',
  templateUrl: './editlophoc.component.html',
  styleUrls: ['./editlophoc.component.scss']
})
export class EditLopHocComponent implements OnInit {
  editLopForm: FormGroup;

  // Danh sách chương trình học
  chuongTrinhList = [
    { id: 1, tenChuongTrinh: 'Chương trình đặc biệt' },
    { id: 2, tenChuongTrinh: 'Chương trình cơ bản' }
  ];

  // Danh sách phòng học
  phongList = [
    { id: 1, tenPhong: 'Phòng 101' },
    { id: 2, tenPhong: 'Phòng 202' },
  ];

  // Danh sách giáo viên
  giaoVienList = [
    { id: 1, tenGiaoVien: 'Bùi Ngọc Dũng' },
    { id: 2, tenGiaoVien: 'Nguyễn Văn A' }
  ];

  // Danh sách học viên có thể chọn từ dropdown
  danhSachHocVienCoSan = [
    { code: 'HE171450', ten: 'Bùi Ngọc Dũng', gioiTinh: 'Nam', ngaySinh: '23-08-2003', email: 'dungbnhe171450@fpt.edu.vn', soDienThoai: '0123-456-789' },
    { code: 'HE171451', ten: 'Nguyễn Văn B', gioiTinh: 'Nam', ngaySinh: '12-04-2002', email: 'nguyenvb@fpt.edu.vn', soDienThoai: '0987-654-321' },
    { code: 'HE171452', ten: 'Trần Thị C', gioiTinh: 'Nữ', ngaySinh: '10-05-2001', email: 'tranthic@fpt.edu.vn', soDienThoai: '0345-678-910' }
  ];

  // Danh sách học viên đã thêm vào lớp
  hocVienList = [];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.editLopForm = this.fb.group({
      tenLop: ['', Validators.required],
      chuongTrinh: [null, Validators.required],
      hocPhi: [null, [Validators.required, Validators.min(400000)]], // Học phí phải >= 400.000
      giaoVien: [null, Validators.required],
      ngayBatDau: ['', [Validators.required, this.validateStartDate]], // Ngày bắt đầu từ hôm nay trở đi
      ngayKetThuc: ['', [Validators.required, this.validateEndDate.bind(this)]], // Ngày kết thúc sau 2 tháng
      lichHoc: this.fb.array([], this.validateDuplicateDays) // Validate tránh trùng thứ
    });
  }

  // Getter lấy danh sách lịch học
  get lichHoc(): FormArray {
    return this.editLopForm.get('lichHoc') as FormArray;
  }
  addHocVien(event: any): void {
    const selectedCode = event.target.value;
    if (!selectedCode) return;

    // Kiểm tra xem học viên đã có trong danh sách chưa
    const selectedHocVien = this.danhSachHocVienCoSan.find(hv => hv.code === selectedCode);
    if (selectedHocVien && !this.hocVienList.some(hv => hv.code === selectedHocVien.code)) {
      this.hocVienList.push(selectedHocVien);
    }

    // Reset dropdown về trạng thái mặc định
    event.target.value = '';
  }
  removeHocVien(hv: any): void {
    this.hocVienList = this.hocVienList.filter(h => h.code !== hv.code);
  }
// Xử lý khi nhấn nút Hủy
  onCancel(): void {
    console.log('Đã hủy chỉnh sửa lớp học.');
  }

  // Xử lý khi nhấn nút Xác nhận
  onSubmit(): void {
    if (this.editLopForm.valid) {
      console.log('Dữ liệu lớp học:', this.editLopForm.value);
    } else {
      this.editLopForm.markAllAsTouched();
    }
  }
  validateDuplicateDays(formArray: AbstractControl): { [key: string]: any } | null {
    const days = formArray.value.map((entry: any) => entry.thu);
    const hasDuplicates = new Set(days).size !== days.length;
    return hasDuplicates ? { duplicateDays: true } : null;
  }

  validateStartDate(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value) return null;
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const startDate = new Date(control.value);
    startDate.setHours(0, 0, 0, 0);

    if (startDate < today) {
      return { invalidStartDate: true };
    }
    return null;
  }

  validateEndDate(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value || !this.editLopForm) return null;
    const startDateControl = this.editLopForm.get('ngayBatDau');
    if (!startDateControl?.value) return null;

    const startDate = new Date(startDateControl.value);
    const minEndDate = new Date(startDate);
    minEndDate.setMonth(minEndDate.getMonth() + 2);

    const endDate = new Date(control.value);
    if (endDate < minEndDate) {
      return { invalidEndDate: true };
    }
    return null;
  }

  validateTimeStart(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value) return null;
    return control.value < '08:00' ? { invalidStartTime: true } : null;
  }

  validateTimeEnd(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value) return null;
    return control.value > '22:00' ? { invalidEndTime: true } : null;
  }
}


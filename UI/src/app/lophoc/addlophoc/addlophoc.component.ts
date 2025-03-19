import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { PhongService } from '../shared/lophoc.service';
@Component({
  selector: 'app-addlophoc',
  templateUrl: './addlophoc.component.html',
  styleUrls: ['./addlophoc.component.scss']
})
export class AddlophocComponent implements OnInit {
  themLopForm: FormGroup;
  phongList: any[] = [];
  // Dữ liệu mẫu, có thể thay bằng dữ liệu từ API
  chuongTrinhList = [
    { id: 1, tenChuongTrinh: 'Chương trình A' },
    { id: 2, tenChuongTrinh: 'Chương trình B' },
  ];
  
  giaoVienList: any[] = [];

  constructor(private fb: FormBuilder,private phongService: PhongService) {}

  ngOnInit(): void {
    this.fetchPhongs(); // Gọi API lấy danh sách phòng
    this.fetchGiaoViens(); // Gọi API lấy danh sách giáo viên
     console.log('Danh sách phòng:', this.phongList);
   
    this.themLopForm = this.fb.group({
      tenLop: ['', [Validators.required, Validators.maxLength(20)]], // Giới hạn 20 ký tự
      chuongTrinh: [null, Validators.required],
      hocPhi: [null, [Validators.required, Validators.min(50000)]], // Học phí phải >= 400.000 
      giaoVien: [null, Validators.required],
      ngayBatDau: ['', [Validators.required, this.validateStartDate]], // Ngày bắt đầu từ hôm nay trở đi
      ngayKetThuc: ['', [Validators.required, this.validateEndDate.bind(this)]], // Ngày kết thúc sau 2 tháng
      lichHoc: this.fb.array([], this.validateDuplicateDays) // Validate tránh trùng thứ
    });

    // Thêm 1 dòng lịch mặc định
    this.addSchedule();
  }
  fetchPhongs(): void {
    this.phongService.getPhongs().subscribe(
      (response) => {
        if (!response.isError) {
          this.phongList = response.data;
          console.log('Danh sách phòng:', this.phongList); 
        } else {
          
        }
      },
      (error) => {
        console.error('Lỗi API:', error);
      }
    );
  }
  fetchGiaoViens(searchTen: string = ''): void {
    const requestPayload = {
      searchTen: searchTen // Có thể truyền giá trị tìm kiếm, nếu không mặc định rỗng
    };
  
    this.phongService.getGiaoViens(requestPayload).subscribe(
      (response) => {
        if (!response.isError) {
          this.giaoVienList = response.data; // Giả sử API trả về danh sách trực tiếp trong response.data
          console.log('Danh sách giáo viên:', this.giaoVienList);
        } else {
          console.error('Lỗi lấy danh sách giáo viên:', response.message);
        }
      },
      (error) => {
        console.error('Lỗi API:', error);
      }
    );
  }
  

  // Tạo getter để truy cập FormArray lichHoc
  get lichHoc(): FormArray {
    return this.themLopForm.get('lichHoc') as FormArray;
  }

  // Hàm tạo form group cho từng lịch học
  createSchedule(): FormGroup {
    return this.fb.group({
      thu: ['', Validators.required], // Bắt buộc chọn thứ
      gioBatDau: ['', [Validators.required, this.validateTimeStart]], // Giờ bắt đầu >= 08:00
      gioKetThuc: ['', [Validators.required, this.validateTimeEnd]], // Giờ kết thúc <= 22:00
      phong: [null, Validators.required] // Bắt buộc chọn phòng
    });
  } 

  // Thêm 1 dòng lịch học
  addSchedule(): void {
  const newSchedule = this.createSchedule();
  
  // Lấy giá trị của các lịch học đã có
  const existingDays = this.lichHoc.controls
    .map(control => control.get('thu')?.value)
    .filter(value => value); // Loại bỏ giá trị null hoặc ""

  // Nếu giá trị mới không phải null hoặc rỗng thì mới kiểm tra trùng lặp
  if (newSchedule.get('thu')?.value && existingDays.includes(newSchedule.get('thu')?.value)) {
    alert('Thứ này đã tồn tại trong lịch học!');
    return;
  }

  this.lichHoc.push(newSchedule);
}


  // Xóa 1 dòng lịch học theo index
  removeSchedule(index: number): void {
    this.lichHoc.removeAt(index);
  }

  // Xử lý khi submit form
  onSubmit(): void {
    if (this.themLopForm.valid) {
      const formValue = this.themLopForm.value;
      console.log('Dữ liệu form:', formValue);
      // Gọi API hoặc xử lý theo yêu cầu
    } else {
      this.themLopForm.markAllAsTouched();
    }
  }

  // Xử lý hủy
  onCancel(): void {
    console.log('Đã hủy thao tác');
    // Logic hủy form hoặc chuyển hướng
  }
  // Validate ngày bắt đầu từ hôm nay trở đi
  validateStartDate(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value) return null;
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Đặt thời gian về 00:00 để so sánh chỉ theo ngày
  
    const startDate = new Date(control.value);
    startDate.setHours(0, 0, 0, 0); // Đặt thời gian về 00:00 để so sánh chính xác
  
    if (startDate < today) {
      return { invalidStartDate: true };
    }
    return null;
  }

  // Validate ngày kết thúc sau ít nhất 2 tháng kể từ ngày bắt đầu
  validateEndDate(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value || !this.themLopForm) return null;
    const startDateControl = this.themLopForm.get('ngayBatDau');
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
  validateDuplicateDays(formArray: AbstractControl): { [key: string]: any } | null {
    const days = formArray.value.map((entry: any) => entry.thu);
    const hasDuplicates = new Set(days).size !== days.length;
    return hasDuplicates ? { duplicateDays: true } : null;
  }

  // Validate giờ bắt đầu >= 08:00
  validateTimeStart(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value) return null; // Không validate nếu chưa nhập
    const startTime = control.value;
    return startTime < '08:00' ? { invalidStartTime: true } : null;
  }

  // Validate giờ kết thúc <= 22:00
  validateTimeEnd(control: AbstractControl): { [key: string]: any } | null {
    if (!control.value) return null;
    const endTime = control.value;
    return endTime > '22:00' ? { invalidEndTime: true } : null;
  }
}


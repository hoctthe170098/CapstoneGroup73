import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { LophocService } from '../shared/lophoc.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
@Component({
  selector: 'app-addlophoc',
  templateUrl: './addlophoc.component.html',
  styleUrls: ['./addlophoc.component.scss']
})
export class AddlophocComponent implements OnInit {
  themLopForm: FormGroup;
  phongList: any[] = [];
  // Dữ liệu mẫu, có thể thay bằng dữ liệu từ API
  chuongTrinhList: any[] = [];
  
  giaoVienList: any[] = [];

  constructor(private fb: FormBuilder,private lophocService: LophocService,private toastr: ToastrService, private router: Router) {}

  ngOnInit(): void {
    this.fetchPhongs(); // Gọi API lấy danh sách phòng
    this.fetchGiaoViens(); // Gọi API lấy danh sách giáo viên
    this.fetchChuongTrinhs();
     
   
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
  fetchChuongTrinhs(): void {
    this.lophocService.getChuongTrinhs().subscribe(
      (res) => {
        if (res.isError) {
          if (res.code === 404) {
            this.router.navigate(['/pages/error']);
          } else {
            this.toastr.error(res.message || 'Đã xảy ra lỗi khi tải lớp học.');
          }
        } else if (!res.isError) {
          this.chuongTrinhList = res.data;
         
        } else {
          
        }
      },
      (err) => {
       
      }
    );
  }
  fetchPhongs(): void {
    this.lophocService.getPhongs().subscribe(
      (response) => {
        if (!response.isError) {
          this.phongList = response.data;
         
        } else {
          
        }
      },
      (error) => {
        
      }
    );
  }
  fetchGiaoViens(searchTen: string = ''): void {
    const requestPayload = {
      searchTen: searchTen // Có thể truyền giá trị tìm kiếm, nếu không mặc định rỗng
    };
  
    this.lophocService.getGiaoViens(requestPayload).subscribe(
      (response) => {
        if (!response.isError) {
          this.giaoVienList = response.data; // Giả sử API trả về danh sách trực tiếp trong response.data
          
        } else {
          
        }
      },
      (error) => {
        
      }
    );
  }
  

  // Tạo getter để truy cập FormArray lichHoc
  get lichHoc(): FormArray {
    return this.themLopForm.get('lichHoc') as FormArray;
  }

  // Hàm tạo form group cho từng lịch học
  createSchedule(): FormGroup {
    const group = this.fb.group({
      thu: ['', Validators.required],
      gioBatDau: ['', [Validators.required, this.validateTimeStart]],
      gioKetThuc: ['', [Validators.required, this.validateTimeEnd]],
      phong: [null, Validators.required]
    }, { validators: this.validateTimeRange }); // 👈 Thêm validator ở đây
  
    return group;
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
  
      const lichHocs = formValue.lichHoc.map((lich: any) => ({
        thu: parseInt(lich.thu),
        phongId: lich.phong.id,
        gioBatDau: lich.gioBatDau,
        gioKetThuc: lich.gioKetThuc
      }));
  
      const payload = {
        lopHocDto: {
          tenLop: formValue.tenLop,
          ngayBatDau: formValue.ngayBatDau,
          ngayKetThuc: formValue.ngayKetThuc,
          hocPhi: formValue.hocPhi,
          giaoVienCode: formValue.giaoVien.code, // đảm bảo có đúng field này
          chuongTrinhId: formValue.chuongTrinh.id,
          lichHocs: lichHocs
        }
      };
  
     
  
      this.lophocService.createLichHocCoDinh(payload).subscribe(
        (res: any) => {
          if (res.isError) {
            this.toastr.error(res.message, 'Lỗi');
          } else {
            this.toastr.success(res.message, 'Thành công');
            this.router.navigate(['/lophoc']);
          }
        },
        (err) => {
          this.toastr.error(err?.error?.message, 'Lỗi');
        }
      );
    } else {
      this.themLopForm.markAllAsTouched();
    }
  }
  
  
  

  // Xử lý hủy
  onCancel(): void {
    this.router.navigate(['/lophoc']);
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
  validateTimeRange(group: AbstractControl): { [key: string]: any } | null {
    const start = group.get('gioBatDau')?.value;
    const end = group.get('gioKetThuc')?.value;
  
    if (!start || !end) return null;
  
    const [startHour, startMinute] = start.split(':').map(Number);
    const [endHour, endMinute] = end.split(':').map(Number);
  
    const startTime = startHour * 60 + startMinute;
    const endTime = endHour * 60 + endMinute;
  
    if (endTime - startTime < 60) {
      return { invalidTimeRange: true };
    }
  
    return null;
  }
  
}


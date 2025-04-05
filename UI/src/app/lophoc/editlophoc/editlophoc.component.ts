import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  FormArray,
} from '@angular/forms';
import { LophocService } from '../shared/lophoc.service';
import { Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-lophoc',
  templateUrl: './editlophoc.component.html',
  styleUrls: ['./editlophoc.component.scss'],
})
export class EditLopHocComponent implements OnInit {
  editLopForm: FormGroup;
  chuongTrinhList: any[] = [];
  giaoVienList: any[] = [];
  phongList: any[] = [];
  hocVienList: any[] = [];
  hocSinhDropdownList: any[] = [];
  canEditAll: boolean = true;
  isEditable: boolean = true;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.editLopForm = this.fb.group({
      tenLop: ['', [Validators.required, Validators.maxLength(20)]],
      chuongTrinh: [null, Validators.required],
      hocPhi: [null, [Validators.required, Validators.min(50000)]],
      giaoVien: [null, Validators.required],
      ngayBatDau: ['', [Validators.required, this.validateStartDateAfterToday()]],
      ngayKetThuc: ['', [Validators.required, this.validateEndDate()]],
      lichHoc: this.fb.array([], this.validateDuplicateDays)
    });
  
    this.loadAllData();
  
    // 👇 Thêm đoạn này để load danh sách học viên cho dropdown
    this.lophocService.searchHocSinh({ searchTen: '' }).subscribe({
      next: (res) => {
        this.hocSinhDropdownList = res?.data || [];
      },
      error: (err) => {
        console.error('❌ Lỗi khi tải danh sách học sinh:', err);
      },
    });
  }

  loadAllData(): void {
    const tenLop = this.route.snapshot.paramMap.get('tenLop');

    Promise.all([
      this.lophocService.getChuongTrinhs().toPromise(),
      this.lophocService.getGiaoViens({ searchTen: '' }).toPromise(),
      this.lophocService.getPhongs().toPromise(),
    ]).then(([chuongTrinhRes, giaoVienRes, phongRes]) => {
      this.chuongTrinhList = chuongTrinhRes?.data || [];
      this.giaoVienList = giaoVienRes?.data || [];
      this.phongList = phongRes?.data || [];

      if (tenLop) {
        this.lophocService.getLopHocByTenLop(tenLop).subscribe({
          next: async (res) => {
            if (res?.data) {
              const today = new Date();
              today.setHours(0, 0, 0, 0);
        
              const startDate = new Date(res.data.ngayBatDau);
              startDate.setHours(0, 0, 0, 0);
        
              this.canEditAll = startDate > today;
              this.isEditable = true; // ✅ Luôn cho phép submit
        
              setTimeout(() => {
                this.patchForm(res.data);
                this.cdr.detectChanges();
              });
            }
          },
          error: () => {
            this.toastr.error('Không thể tải lớp học theo tên.');
          },
        });
        
      }
    });
  }

  patchForm(data: any): void {
    const chuongTrinhObj = this.chuongTrinhList.find(ct => ct.id === data.chuongTrinhId);
    const giaoVienObj = this.giaoVienList.find(gv => gv.code === data.giaoVienCode);
  
    this.editLopForm.patchValue({
      tenLop: data.tenLop,
      chuongTrinh: chuongTrinhObj || null,
      hocPhi: data.hocPhi,
      giaoVien: giaoVienObj || null,
      ngayBatDau: data.ngayBatDau?.substring(0, 10),
      ngayKetThuc: data.ngayKetThuc?.substring(0, 10),
    });
  
    const lichHocArray = this.editLopForm.get('lichHoc') as FormArray;
    lichHocArray.clear();
  
    data.lichHocs.forEach((lich: any) => {
      const phongObj = this.phongList.find(p => p.id === lich.phongId);
  
      lichHocArray.push(
        this.fb.group({
          id: [lich.id],
          thu: [lich.thu, Validators.required],
          gioBatDau: [lich.gioBatDau, [Validators.required, this.validateTimeStart]],
          gioKetThuc: [lich.gioKetThuc, [Validators.required, this.validateTimeEnd]],
          phong: [phongObj || null, Validators.required],
        })
      );
    });
  
    this.hocVienList = data.hocSinhs || [];
  
    if (!this.canEditAll) {
      this.editLopForm.get('tenLop')?.disable();
      this.editLopForm.get('chuongTrinh')?.disable();
      this.editLopForm.get('hocPhi')?.disable();
      this.editLopForm.get('giaoVien')?.disable();
      this.editLopForm.get('ngayKetThuc')?.disable();
  
      this.lichHoc.controls.forEach(control => {
        control.get('thu')?.disable();
        control.get('gioBatDau')?.disable();
        control.get('gioKetThuc')?.disable();
      });
    }
  }
  

  get lichHoc(): FormArray {
    return this.editLopForm.get('lichHoc') as FormArray;
  }

  onCancel(): void {
    console.log('❌ Hủy chỉnh sửa.');
  }

  onSubmit(): void {
   
    
  
    const formValue = this.editLopForm.getRawValue();
  
    const payload = {
      lopHocDto: {
        tenLop: formValue.tenLop,
        giaoVienCode: formValue.giaoVien?.code || '',
        ngayBatDau: this.formatDate(formValue.ngayBatDau),
        ngayKetThuc: this.formatDate(formValue.ngayKetThuc),
        hocPhi: formValue.hocPhi,
        chuongTrinhId: formValue.chuongTrinh?.id || 0,
        lichHocs: formValue.lichHoc.map((lich: any) => ({
          id: lich.id || null,
          thu: lich.thu,
          phongId: lich.phong?.id || 0,
          gioBatDau: lich.gioBatDau,
          gioKetThuc: lich.gioKetThuc,
        })),
        hocSinhCodes: this.hocVienList.map((hv: any) => hv.code),
      },
    };
  
   
  
    this.lophocService.editLichHoc(payload).subscribe({
      next: (res) => {
        if (res?.isError) {
          this.toastr.error(res.message);
        } else {
          this.toastr.success(res.message);
        }
      },
      error: (err) => {
        this.toastr.error('Lỗi khi gửi dữ liệu đến máy chủ.');
      },
    });
  }
  
  
  

  removeHocVien(index: number): void {
    this.hocVienList.splice(index, 1);
  }

  removeSchedule(index: number): void {
    if (this.canEditAll) {
      this.lichHoc.removeAt(index);
    }
  }
  validateEndDate() {
    return (control: AbstractControl): ValidationErrors | null => {
      const start = new Date(this.editLopForm?.get('ngayBatDau')?.value);
      const end = new Date(control.value);
      const twoMonthsLater = new Date(start);
      twoMonthsLater.setMonth(twoMonthsLater.getMonth() + 2);
  
      return end < twoMonthsLater ? { invalidEndDate: true } : null;
    };
  }
  validateStartDateAfterToday() {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
  
      const inputDate = new Date(control.value);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
  
      return inputDate <= today ? { invalidStartDate: true } : null;
    };
  }
   

  addSchedule(): void {
    if (this.canEditAll) {
      const lichHocArray = this.editLopForm.get('lichHoc') as FormArray;
      lichHocArray.push(this.fb.group({
        thu: ['', Validators.required], // Bắt buộc chọn thứ
        gioBatDau: ['', [Validators.required, this.validateTimeStart]], // Giờ bắt đầu >= 08:00
        gioKetThuc: ['', [Validators.required, this.validateTimeEnd]], // Giờ kết thúc <= 22:00
        phong: [null, Validators.required] // Bắt buộc chọn phòng
      }));
    }
  }
  private formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toISOString().split('T')[0]; // Trả về dạng yyyy-MM-dd
  }
  onSelectHocSinh(code: string): void {
    const selected = this.hocSinhDropdownList.find(hs => hs.code === code);
    if (selected && !this.hocVienList.find(hv => hv.code === code)) {
      this.hocVienList.push(selected);
    }
  
    // 👉 Reset dropdown
    const selectElement = document.getElementById('hocSinhDropdown') as HTMLSelectElement;
    if (selectElement) {
      selectElement.value = '';
    }
  }
  validateDuplicateDays(formArray: AbstractControl): { [key: string]: any } | null {
    const days = formArray.value.map((entry: any) => entry.thu);
    const hasDuplicates = new Set(days).size !== days.length;
    return hasDuplicates ? { duplicateDays: true } : null;
  }
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

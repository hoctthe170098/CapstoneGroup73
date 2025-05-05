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
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';

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
  hocSinhDropdownOpen: boolean = false;
hocSinhSearchTerm: string = '';
hocSinhDropdownList: any[] = []; // dữ liệu từ API
filteredHocSinhList: any[] = [];
selectedHocSinh: any = null;
  canEditAll: boolean = true;
  isEditable: boolean = true;
  searchHocSinhTen: string = '';
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService,
    private router: Router,
    private spinner:NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.editLopForm = this.fb.group({
      tenLop: ['', [Validators.required, Validators.maxLength(20)]],
      chuongTrinh: [null, Validators.required],
      hocPhi: [null, [Validators.required, Validators.min(50000), Validators.max(500000)]],
      giaoVien: [null, Validators.required],
      ngayBatDau: ['', [Validators.required]],
      ngayKetThuc: ['', [Validators.required, this.validateEndDate()]],
      lichHoc: this.fb.array([], [this.validateDuplicateDays.bind(this)])
    });
    this.lichHoc.valueChanges.subscribe(() => {
      this.lichHoc.updateValueAndValidity({ onlySelf: false });
    });
    
    this.loadAllData();
    this.loadHocSinhList();
  
    
    this.lophocService.searchHocSinh({ searchTen: '' }).subscribe({
      next: (res) => {
        this.hocSinhDropdownList = res?.data || [];
      },
      error: (err) => {
        console.error('Lỗi khi tải danh sách học sinh:', err);
      },
    });
  }
  loadHocSinhList() {
    this.lophocService.searchHocSinh({ searchTen: '' }).subscribe({
      next: (res) => {
        this.hocSinhDropdownList = res?.data || [];
        this.filteredHocSinhList = this.hocSinhDropdownList.slice();
      },
      error: (err) => {
        this.toastr.error('Không thể tải danh sách học sinh');
      }
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
            if (res.isError) {
              if (res.code === 404) {
                this.router.navigate(['/pages/error']);
              } else {
                this.toastr.error(res.message || 'Đã xảy ra lỗi khi tải lớp học.');
              }
            } else if (res?.data) {
              const today = new Date();
              today.setHours(0, 0, 0, 0);
        
              const startDate = new Date(res.data.ngayBatDau);
              startDate.setHours(0, 0, 0, 0);
        
              this.canEditAll = startDate > today;
              this.isEditable = true; //  Luôn cho phép submit
        
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
  
      const group = this.fb.group({
        id: [lich.id],
        thu: [lich.thu, Validators.required],
        gioBatDau: [lich.gioBatDau, [Validators.required, this.validateTimeStart]],
        gioKetThuc: [lich.gioKetThuc, [Validators.required, this.validateTimeEnd]],
        phong: [phongObj || null, Validators.required],
      }, { validators: this.validateTimeRange });
  
      //  Lắng nghe thay đổi của field 'thu'
      group.valueChanges.subscribe(() => {
        group.markAsTouched({ onlySelf: false });
        group.markAsDirty({ onlySelf: false });
        this.lichHoc.updateValueAndValidity({ onlySelf: false });
      });
      
  
      lichHocArray.push(group);
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
    this.router.navigate(['/lophoc']);
  }

  onSubmit(): void {
    if (this.editLopForm.invalid) {
      this.editLopForm.markAllAsTouched();
      this.lichHoc.markAsTouched();
      if (this.editLopForm.invalid || this.lichHoc.hasError('duplicateDays')) {
        return;
      }
    }
    
  
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
  
   
  this.spinner.show();
    this.lophocService.editLichHoc(payload).subscribe({
      next: (res) => {
        this.spinner.hide();
        if (res?.isError) {
          this.toastr.error(res.message);
        } else {
          this.toastr.success(res.message);
          this.router.navigate(['/lophoc']);
        }
      },
      error: (err) => {
        this.spinner.hide();
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
      const group = this.fb.group({
        thu: ['', Validators.required],
        gioBatDau: ['', [Validators.required, this.validateTimeStart]],
        gioKetThuc: ['', [Validators.required, this.validateTimeEnd]],
        phong: [null, Validators.required],
      }, { validators: this.validateTimeRange });
  
      //Khi bất kỳ trường nào trong group thay đổi thì chạy lại toàn bộ validator array
      group.get('thu')?.valueChanges.subscribe(() => {
        this.lichHoc.updateValueAndValidity(); //  BẮT BUỘC
      });
  
      this.lichHoc.push(group);
  
      //  Force chạy validator khi thêm mới
      this.lichHoc.updateValueAndValidity();
    }
  }
  
  private formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toISOString().split('T')[0]; // Trả về dạng yyyy-MM-dd
  }
  onThuChange(): void {
    //  đánh dấu dirty/touched từng dòng
    this.lichHoc.controls.forEach((ctrl) => {
      ctrl.markAsTouched();
      ctrl.markAsDirty();
    });
  
    //  luôn ép validator chạy lại sau khi có thay đổi
    this.lichHoc.updateValueAndValidity();
  }
  isDuplicate(value: string, currentIndex: number): boolean {
    const controls = this.lichHoc.controls;
    const thuList = controls
      .map((ctrl, i) => i !== currentIndex ? String(ctrl.get('thu')?.value) : null)
      .filter(v => !!v);
  
    return thuList.includes(String(value));
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
  validateDuplicateDays(formArray: AbstractControl): ValidationErrors | null {
    const controls = (formArray as FormArray).controls;
    const thuList: string[] = controls
      .map(control => String(control.get('thu')?.value)) // ép về string
      .filter(v => !!v);
  
    const hasDuplicate = new Set(thuList).size !== thuList.length;
  
    console.log('[DEBUG] Danh sách "thu" ép kiểu:', thuList);
  
    return hasDuplicate ? { duplicateDays: true } : null;
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
  validateTimeRange(group: AbstractControl): { [key: string]: any } | null {
    const start = group.get('gioBatDau')?.value;
    const end = group.get('gioKetThuc')?.value;
  
    if (!start || !end) return null;
  
    const [startHour, startMinute] = start.split(':').map(Number);
    const [endHour, endMinute] = end.split(':').map(Number);
  
    const startTime = startHour * 60 + startMinute;
    const endTime = endHour * 60 + endMinute;
  
    if (endTime - startTime < 120) {
      return { invalidTimeRange: true };
    }
  
    return null;
  }
  onHocSinhSearchTermChange() {
    this.hocSinhDropdownOpen = true; //  luôn mở khi gõ
  
    const term = this.hocSinhSearchTerm.toLowerCase().trim();
    this.filteredHocSinhList = this.hocSinhDropdownList.filter(hs =>
      hs.ten.toLowerCase().includes(term) || hs.code.toLowerCase().includes(term)
    );
  }
  
  
  //  Mở/đóng dropdown khi click
  toggleHocSinhDropdown() {
    this.hocSinhDropdownOpen = !this.hocSinhDropdownOpen;
    this.hocSinhSearchTerm = '';
    this.filteredHocSinhList = this.hocSinhDropdownList.slice();
  }
  
  //  Chọn học sinh từ dropdown
  selectHocSinh(hs: any) {
    if (!this.hocVienList.find(hv => hv.code === hs.code)) {
      this.hocVienList.push(hs);
    }
    this.selectedHocSinh = hs;
    this.hocSinhDropdownOpen = false;
  }
  
  
}

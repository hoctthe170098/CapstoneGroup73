import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  FormArray,
} from '@angular/forms';
import { LophocService } from '../shared/lophoc.service';
import { Validators, AbstractControl, ValidationErrors } from '@angular/forms';

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

  canEditAll: boolean = true;
  isEditable: boolean = true;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.editLopForm = this.fb.group({
      tenLop: ['', [Validators.required, Validators.maxLength(20)]],
      chuongTrinh: [null, Validators.required],
      hocPhi: [null, [Validators.required, Validators.min(50000)]],
      giaoVien: [null, Validators.required],
      ngayBatDau: ['', [Validators.required, this.validateStartDateAfterToday()]],
      ngayKetThuc: ['', [Validators.required, this.validateEndDate()]],
      lichHoc: this.fb.array([]),
    });
    

    this.loadAllData();
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
                    today.setHours(0, 0, 0, 0); // reset về đầu ngày

              const startDate = new Date(res.data.ngayBatDau);
                    startDate.setHours(0, 0, 0, 0);

              this.canEditAll = startDate > today;

              this.isEditable = true;

              setTimeout(() => {
                this.patchForm(res.data);
                this.cdr.detectChanges();
              });
            }
          },
          error: (err) => {
            console.error('❌ Không thể tải lớp học theo tên:', err);
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
          thu: [lich.thu],
          gioBatDau: [lich.gioBatDau],
          gioKetThuc: [lich.gioKetThuc],
          phong: [phongObj || null],
        })
      );
    });

    this.hocVienList = data.hocSinhs || [];

    // Disable fields if cannot edit all
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
    if (!this.isEditable || this.editLopForm.invalid) {
      console.warn('⛔ Không thể gửi vì không hợp lệ hoặc không được phép chỉnh sửa.');
      return;
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
  
    console.log('📦 Payload gửi đi:', payload); // 👉 log ở đây
  
    this.lophocService.editLichHoc(payload).subscribe({
      next: (res) => {
        console.log('✅ Cập nhật lớp học thành công:', res);
      },
      error: (err) => {
        console.error('❌ Lỗi khi cập nhật lớp học:', err);
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
        thu: [''],
        gioBatDau: [''],
        gioKetThuc: [''],
        phong: [null],
      }));
    }
  }
  private formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toISOString().split('T')[0]; // Trả về dạng yyyy-MM-dd
  }
}

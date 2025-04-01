import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LophocService } from '../shared/lophoc.service';
import { ChangeDetectorRef } from '@angular/core';
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
  hocVienList = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.editLopForm = this.fb.group({
      tenLop: [''],
      chuongTrinh: [null],
      hocPhi: [null],
      giaoVien: [null],
      ngayBatDau: [''],
      ngayKetThuc: [''],
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
          next: (res) => {
            if (res?.data) {
              // Đợi 1 chút để DOM render xong select option
              setTimeout(() => {
                this.patchForm(res.data);
                this.cdr.detectChanges(); // ép render lại
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
          thu: [lich.thu],
          gioBatDau: [lich.gioBatDau],
          gioKetThuc: [lich.gioKetThuc],
          phong: [phongObj || null],
        })
      );
    });

    this.hocVienList = data.hocSinhs || [];
  }

  get lichHoc(): FormArray {
    return this.editLopForm.get('lichHoc') as FormArray;
  }

  onCancel(): void {
    console.log('Đã hủy chỉnh sửa lớp học.');
  }

  onSubmit(): void {
    console.log('Dữ liệu lớp học:', this.editLopForm.value);
  }

  // Compare functions (giữ lại nếu cần xài trong HTML template)
  compareById(o1: any, o2: any): boolean {
    return o1 && o2 && o1.id === o2.id;
  }

  compareByCode(o1: any, o2: any): boolean {
    return o1 && o2 && o1.code === o2.code;
  }
}

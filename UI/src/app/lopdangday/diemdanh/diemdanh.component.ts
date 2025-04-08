import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LopdangdayService } from '../shared/lopdangday.service';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-diemdanh',
  templateUrl: './diemdanh.component.html',
  styleUrls: ['./diemdanh.component.scss']
})
export class DiemdanhComponent implements OnInit {
  students: any[] = [];
  tenLop: string = '';
  isError: boolean = false;
  thongBao: string = '';

  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private lopService: LopdangdayService,
    private router: Router,
    private location: Location,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      const tenLopParam = params.get('tenLop');
      if (tenLopParam) {
        this.tenLop = tenLopParam;
        this.loadDiemDanh(this.tenLop);
      }
    });
  }

  loadDiemDanh(tenLop: string) {
    this.lopService.getDiemDanhTheoNgay(tenLop).subscribe(
      (res) => {
        if (res.isError) {
          this.students = [];
          this.isError = true;
          if (res.code === 404) {
            this.router.navigate(['/pages/error'])
            return;
          }

          this.thongBao = res.message;
          this.cdr.detectChanges();
          return;
        }

        if (!res.data || res.data.length === 0) {
          this.students = [];
          this.isError = false;
          this.thongBao = 'Không có học sinh nào được điểm danh hôm nay.';
          return;
        }

        this.isError = false;
        this.thongBao = '';
        this.tenLop = res.data[0].tenLop || this.tenLop;

        this.students = res.data.map(sv => ({
          id: sv.id, 
          code: sv.hocSinhCode,
          name: sv.tenHocSinh,
          diemDanh: sv.trangThai === 'Có mặt' ? 'coMat' : 'vang',
          diemBTVN: sv.diemBTVN ?? '',
          diemTrenLop: sv.diemTrenLop ?? '',
          nhanXet: sv.nhanXet ?? ''
        }));

        this.cdr.detectChanges();
      },
      (err) => {
        this.students = [];
        this.isError = true;
        if (err.status === 404 || err.error?.code === 404) {
          this.location.back();
          return;
        }
        this.thongBao = err.error?.message;
       
      }
    );
  }

  onSubmit() {
    const payload = {
      updateDiemDanhs: this.students.map(student => {
        return {
          id: student.id,
          hocSinhCode: student.code,
          tenHocSinh: student.name,
          trangThai: student.diemDanh === 'coMat' ? 'Có mặt' : 'Vắng',
          diemBTVN: this.formatScore(student.diemBTVN),
          diemTrenLop: this.formatScore(student.diemTrenLop),
          nhanXet: student.nhanXet?.toString().substring(0, 200) || ''
        };
      })
    };
  
    
  
    this.lopService.updateDiemDanhTheoNgay(payload).subscribe({
      next: (res) => {
        this.toastr.success('Lưu thành công!');
        this.loadDiemDanh(this.tenLop); // reload để đồng bộ
      },
      error: (err) => {
       
        alert(` ${err.error?.message || 'Không thể cập nhật điểm danh.'}`);
      }
    });
  }
  
  formatScore(score: any): number {
    const n = Number(score);
    if (isNaN(n) || n < 0) return 0;
    if (n > 10) return 10;
    return Math.round(n * 10) / 10;
  }

  getDisplayScore(score: string): string {
    return score ? `${score} / 10` : '';
  }

  updateScore(value: string, student: any, field: 'diemBTVN' | 'diemTrenLop') {
    const trimmed = value.split('/')[0].trim();
    student[field] = trimmed;
  }
}

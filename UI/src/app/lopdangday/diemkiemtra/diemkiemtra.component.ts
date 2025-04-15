import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { KetQuaBaiKiemTra } from '../shared/lopdangday.model';
import { LopdangdayService } from '../shared/lopdangday.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-diemkiemtra',
  templateUrl: './diemkiemtra.component.html',
  styleUrls: ['./diemkiemtra.component.scss']
})
export class DiemkiemtraComponent implements OnInit {
  danhSachHocSinh: KetQuaBaiKiemTra[] = [];
  baiKiemTraId: string = '';
  tenBaiKiemTra: string = '';
  ngayKiemTra: string = '';

  validationErrors: Record<string, { diem?: string; nhanXet?: string }> = {};

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private lopdangdayService: LopdangdayService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('baiKiemTraId');
      if (id) {
        this.baiKiemTraId = id;
        this.loadKetQua();
      }
    });
  }

  loadKetQua() {
    this.lopdangdayService.getKetQuaBaiKiemTraChoGiaoVien(this.baiKiemTraId).subscribe({
      next: res => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error']);
          return;
        }
        this.tenBaiKiemTra = res.data.tenBaiKiemTra;
        this.ngayKiemTra = res.data.ngayKiemTra;
        this.danhSachHocSinh = res.data.ketQuaBaiKiemTra;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
      }
    });
  }

  luuKetQua() {
    this.validationErrors = {}; // reset lỗi
    let hasError = false;

    this.danhSachHocSinh.forEach(hs => {
      const errors: { diem?: string; nhanXet?: string } = {};

      const diem = typeof hs.diem === 'string' ? parseFloat(hs.diem) : hs.diem;

      if (diem !== null && (diem < 0 || diem > 10)) {
        errors.diem = 'Điểm phải từ 0 đến 10';
        hasError = true;
      }
      console.log("0123456789012345678asd2345678901234...1111".length);
      console.log(`=> Nhận xét của ${hs.hocSinhCode}:`, hs.nhanXet);
console.log(`=> Độ dài:`, (hs.nhanXet ?? '').toString().length);
      if ((hs.nhanXet ?? '').toString().length > 200) {
        errors.nhanXet = 'Nhận xét không được vượt quá 200 ký tự';
        hasError = true;
      }
      

      if (Object.keys(errors).length > 0) {
        this.validationErrors[hs.hocSinhCode] = errors;
      }
    });

    if (hasError) return; // không gửi nếu có lỗi

    const dataToSend = this.danhSachHocSinh.map(hs => ({
      id: hs.id,
      hocSinhCode: hs.hocSinhCode || '',
      tenHocSinh: hs.tenHocSinh,
      diem: typeof hs.diem === 'string' ? parseFloat(hs.diem) : hs.diem,
      nhanXet: hs.nhanXet
    }));

    const payload = { updateKetQuas: dataToSend };

    this.lopdangdayService.updateKetQuaBaiKiemTra(payload).subscribe({
      next: res => {
        if (res?.isError) {
          this.toastr.error(res.message || 'Có lỗi xảy ra!');
        } else {
          this.toastr.success(res.message || 'Lưu kết quả thành công!');
        }
      },
      error: err => {
        console.error('Lỗi khi lưu:', err);
        this.toastr.error('Lỗi hệ thống hoặc không thể kết nối.');
      }
    });
  }
}
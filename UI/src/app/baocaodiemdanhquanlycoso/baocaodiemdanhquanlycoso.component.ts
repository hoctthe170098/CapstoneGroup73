import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { BaocaodiemdanhquanlycosohService } from './shared/baocaodiemdanhquanlycoso.service';
import { formatDate } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-baocaodiemdanhquanlycoso',
  templateUrl: './baocaodiemdanhquanlycoso.component.html',
  styleUrls: ['./baocaodiemdanhquanlycoso.component.scss']
})
export class BaocaodiemdanhquanlycosoComponent implements OnInit {
  tenLop: string = '';
  danhSachNgay: string[] = [];
  ngayDuocChon: string = '';
  danhSachHocSinh: any[] = [];
  backupDanhSach: any[] = [];
  dangSua = false;

  constructor(
    private diemDanhService: BaocaodiemdanhquanlycosohService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const tenLop = params['TenLop'];
      if (tenLop) {
        this.tenLop = tenLop;
        this.taiBaoCao();
      }
    });
  }

  taiBaoCao(ngay?: string) {
    this.diemDanhService.getBaoCaoDiemDanh(this.tenLop, ngay).subscribe(res => {
      const data = res.data;
      this.danhSachNgay = data.ngays.map((d: string) => this.formatDateOnly(d));
      this.ngayDuocChon = this.formatDateOnly(data.ngay);
      this.danhSachHocSinh = data.diemDanhs.map((dd: any) => ({
        id: dd.id,
        code: dd.hocSinhCode,
        ten: dd.tenHocSinh,
        trangThai: dd.trangThai
      }));
      this.backupDanhSach = JSON.parse(JSON.stringify(this.danhSachHocSinh));
      this.toastr.success
      this.cdr.detectChanges();
    });
  }

  chonNgay(date: string) {
    this.ngayDuocChon = date;
    this.dangSua = false;
    const ngayParam = this.toApiDateFormat(date);
    this.taiBaoCao(ngayParam);
  }

  huySua() {
    this.danhSachHocSinh = JSON.parse(JSON.stringify(this.backupDanhSach));
    this.dangSua = false;
    this.cdr.detectChanges();
  }

  luuDiemDanh() {
    const payload = {
      updateDiemDanhs: this.danhSachHocSinh.map(hs => ({
        id: hs.id,
        hocSinhCode: hs.code,
        trangThai: hs.trangThai
      }))
    };

    this.diemDanhService.updateDiemDanh(payload).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.backupDanhSach = JSON.parse(JSON.stringify(this.danhSachHocSinh));
          this.dangSua = false;

          this.cdr.detectChanges();
          this.toastr.success('Cập nhật điểm danh thành công!', 'Thành công');
        } else {
          // Xử lý lỗi từ server
        }
      },
      error: (err) => {
        // Xử lý lỗi kết nối hoặc lỗi khác
        this.dangSua = false;
        this.cdr.detectChanges();
      }
    });
  }

  private formatDateOnly(dateStr: string): string {
    const d = new Date(dateStr);
    const weekdays = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
    const thu = weekdays[d.getDay()];
    return `${thu}, ${formatDate(d, 'dd/MM/yyyy', 'en-US')}`;
  }

  private toApiDateFormat(ngayStr: string): string {
    const parts = ngayStr.split(', ')[1].split('/');
    return `${parts[2]}-${parts[1]}-${parts[0]}`;
  }
}

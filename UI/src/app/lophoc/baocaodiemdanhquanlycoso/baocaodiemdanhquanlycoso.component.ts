import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LophocService } from '../shared/lophoc.service';
import { formatDate } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
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
    private lophocService: LophocService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService,
    private router: Router,
    private spinner: NgxSpinnerService
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
    this.spinner.show();
    this.lophocService.getBaoCaoDiemDanh(this.tenLop, ngay).subscribe(res => {
      if(res.isError){
        this.spinner.hide();
        if(res.code==404) this.router.navigate(['/pages/error']);
        else this.toastr.error(res.message);
      }else{
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
      }    
    }, err => {
      this.spinner.hide(); 
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
      updateDiemDanhs: this.danhSachHocSinh
        .filter(hs => hs.id !== '00000000-0000-0000-0000-000000000000')
        .map(hs => ({
          id: hs.id,
          hocSinhCode: hs.code,
          trangThai: hs.trangThai
        }))
    };
    if (payload.updateDiemDanhs.length === 0) {
      this.toastr.error('Không có học sinh nào được chọn để cập nhật điểm danh.');
      return; 
    }
    this.spinner.show();
    this.lophocService.updateDiemDanh(payload).subscribe({
      next: (res) => {
        this.spinner.hide();
        if (!res.isError) {
          this.backupDanhSach = JSON.parse(JSON.stringify(this.danhSachHocSinh));
          this.dangSua = false;
          this.cdr.detectChanges();
          this.toastr.success('Cập nhật điểm danh thành công!', 'Thành công');
        } else {
          this.toastr.error(res.message);
        }
      },
      error: (err) => {
        this.spinner.hide();
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

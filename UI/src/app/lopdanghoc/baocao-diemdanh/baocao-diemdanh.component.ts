import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { LopdanghocService } from '../shared/lopdanghoc.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-baocao-diemdanh',
  templateUrl: './baocao-diemdanh.component.html',
  styleUrls: ['./baocao-diemdanh.component.scss']
})
export class BaocaoDiemdanhComponent implements OnInit {
  baoCaoDiemDanh: {
    ngay: string;
    thoiGian: string;
    phong: string;
    giaoVien: string;
    trangThai: string;
  }[] = [];

  constructor(
    private lopService: LopdanghocService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      const tenLop = params.get('tenLop');
      if (tenLop) {
        this.layDuLieuDiemDanh(tenLop);
        
      }
    });
  }

  layDuLieuDiemDanh(tenLop: string): void {
  
    this.lopService.getBaoCaoDiemDanh(tenLop).subscribe({
      next: (res) => {
        
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        
        if (res?.data?.length) {
          this.baoCaoDiemDanh = res.data.map((item: any) => ({
            ngay: this.formatNgay(item.ngay),
            thoiGian: `${item.thoiGianBatDau} - ${item.thoiGianKetThuc}`,
            phong: item.tenPhong,
            giaoVien: item.tenGiaoVien,
            trangThai: item.tinhTrangDiemDanh
          }));
        } else {
          this.baoCaoDiemDanh = [];
          
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
      }
      
    });
  }

  private formatNgay(ngay: string): string {
    const d = new Date(ngay);
    const dd = d.getDate().toString().padStart(2, '0');
    const mm = (d.getMonth() + 1).toString().padStart(2, '0');
    const yyyy = d.getFullYear();
    return `${dd}/${mm}/${yyyy}`;
  }
}

import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { LopdanghocService } from 'app/lopdanghoc/shared/lopdanghoc.service';
import { ActivatedRoute,Router } from '@angular/router';

@Component({
  selector: 'app-baocao-diem',
  templateUrl: './baocao-diem.component.html',
  styleUrls: ['./baocao-diem.component.scss']
})
export class BaocaoDiemComponent implements OnInit {
  tenLop: string = '';
  diemTrungBinh = {
    diemTrenLop: 0,
    diemBaiTap: 0,
    diemKiemTra: 0
  };
  
  nhanXetDinhKy: { ngay: string; nhanXet: string }[] = [];
  diemHangNgay: { ngay: string; diemTrenLop: string; diemBTVN: string; nhanXet: string }[] = [];
  diemKiemTras: any[] = [];

  constructor(
    private lopdanghocService: LopdanghocService,
    private route: ActivatedRoute, 
    private cdr: ChangeDetectorRef,
    private router:Router
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
      if (this.tenLop) {
        this.loadBaoCaoDiem(this.tenLop);
      }
    });
  }

  loadBaoCaoDiem(tenLop: string): void {
    this.lopdanghocService.getBaoCaoTatCaCacDiem(tenLop).subscribe({
      next: (res) => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        if (!res.isError && res.data) {
          const data = res.data;
          this.diemTrungBinh = {
            diemTrenLop: data.diemTrenLopTB,
            diemBaiTap: data.diemBaiTapTB,
            diemKiemTra: data.diemKiemTraTB
          };
          this.nhanXetDinhKy = data.nhanXetDinhKy || [];
          this.diemHangNgay = data.diemHangNgay || [];
          this.diemKiemTras = data.diemKiemTras || [];
          this.cdr.detectChanges();
        }

      },
      error: (err) => {
        console.error('Lỗi khi gọi API:', err);
      }
    });
  }
}

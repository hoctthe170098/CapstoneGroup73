import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { LopdanghocService } from '../shared/lopdanghoc.service';
import { ActivatedRoute,Router } from '@angular/router';
@Component({
  selector: 'app-lich-thi',
  templateUrl: './lich-thi.component.html',
  styleUrls: ['./lich-thi.component.scss']
})
export class LichThiComponent implements OnInit {
  lichThi: any[] = [];
  tenLop: string = '';

  constructor(
    private lopdanghocService: LopdanghocService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr:ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
      this.getLichKiemTra();
    });
  }

  getLichKiemTra(): void {
    this.lopdanghocService.getLichKiemTraVaKetQua(this.tenLop).subscribe({
      next: (res) => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        this.lichThi = res.data || [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi khi lấy lịch kiểm tra:', err);
      }
    });
  }
}

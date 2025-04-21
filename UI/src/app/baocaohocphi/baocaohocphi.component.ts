import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { BaocaohocphiService } from './shared/baocaohocphi.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-baocaohocphi',
  templateUrl: './baocaohocphi.component.html',
  styleUrls: ['./baocaohocphi.component.scss']
})
export class BaocaohocphiComponent implements OnInit {
  baoCaoHocPhi: any;
  danhSachThang: number[] = [];
  selectedThang?: number;
  tenLop: string = ''; 

  constructor(
    private baoCaoHocPhiService: BaocaohocphiService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const tenLop = params['TenLop'];
      if (tenLop) {
        this.tenLop = tenLop;
        this.loadBaoCao(); // gọi API luôn khi có tên lớp
      }
    });
  }
  loadBaoCao(thang?: number): void {
    this.baoCaoHocPhiService.getBaoCaoHocPhi(this.tenLop, thang).subscribe(res => {
      this.baoCaoHocPhi = res.data;
      this.danhSachThang = [];
      for (let i = this.baoCaoHocPhi.thangBatDau; i <= this.baoCaoHocPhi.thangKetThuc; i++) {
        this.danhSachThang.push(i);
      }
      this.selectedThang = this.baoCaoHocPhi.thang;
      this.cdr.detectChanges();
    });
  }

  onThangChange(): void {
    this.loadBaoCao(this.selectedThang);
  }
}

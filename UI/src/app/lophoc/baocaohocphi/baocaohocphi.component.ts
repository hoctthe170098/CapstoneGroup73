import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { LophocService } from '../shared/lophoc.service';
import { ActivatedRoute,Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
    private lophocService: LophocService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService,
    private router: Router
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
    this.lophocService.getBaoCaoHocPhi(this.tenLop, thang).subscribe(res => {
      if (res.isError) {
        if (res.code === 404) {
          this.router.navigate(['/pages/error']);
        } else {
          this.toastr.error(res.message);
        }
      } else {
        this.baoCaoHocPhi = res.data;
        this.danhSachThang = [];
        for (let i = this.baoCaoHocPhi.thangBatDau; i <= this.baoCaoHocPhi.thangKetThuc; i++) {
          this.danhSachThang.push(i);
        }
        this.selectedThang = this.baoCaoHocPhi.thang;
        this.cdr.detectChanges();
      }
    });
  }
  

  onThangChange(): void {
    this.loadBaoCao(this.selectedThang);
  }
}

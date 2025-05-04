import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { LophocService } from '../shared/lophoc.service';
import { ActivatedRoute,Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
@Component({
  selector: 'app-baocaohocphi',
  templateUrl: './baocaohocphi.component.html',
  styleUrls: ['./baocaohocphi.component.scss']
})
export class BaocaohocphiComponent implements OnInit {
  baoCaoHocPhi: any;
  danhSachThang: { thang: number; nam: number }[] = [];
  selectedThangNam: { thang: number; nam: number } | null = null;
  tenLop: string = ''; 
  
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
        this.loadBaoCao(); // gọi API luôn khi có tên lớp
      }
    });
  }
  loadBaoCao(thang?: number, nam?: number): void {
    this.spinner.show();
    this.lophocService.getBaoCaoHocPhi(this.tenLop, thang, nam).subscribe(res => {
      if (res.isError) {
        this.spinner.hide();
        if (res.code === 404) {
          this.router.navigate(['/pages/error']);
        } else {
          this.toastr.error(res.message);
        }
      } else {
        this.baoCaoHocPhi = res.data;
  
        // map danh sách tháng-năm
        this.danhSachThang = res.data.thangNams.map((tn: any) => ({
          thang: tn.thang,
          nam: tn.nam
        }));
  
        // gán selected ban đầu
        this.danhSachThang = res.data.thangNams.map((tn: any) => ({
          thang: tn.thang,
          nam: tn.nam
        }));
        
        const selected = this.danhSachThang.find(
          t => t.thang === res.data.thang && t.nam === res.data.nam
        );
        this.selectedThangNam = selected ?? null;
  
        this.cdr.detectChanges();
      }
    }, err => {
      this.spinner.hide();
      this.toastr.error('Lỗi khi tải báo cáo học phí!');
    });
  }
  
  

  onThangNamChange(): void {
    if (this.selectedThangNam) {
      this.loadBaoCao(this.selectedThangNam.thang, this.selectedThangNam.nam);
    }
  }
  
}

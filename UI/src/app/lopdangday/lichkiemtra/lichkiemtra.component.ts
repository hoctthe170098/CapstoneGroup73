import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LopdangdayService } from '../shared/lopdangday.service';
interface BaiKiemTraDto {
  id: string;
  ten: string;
  urlFile: string;
  ngayKiemTra: string;
  trangThai: string;
}

@Component({
  selector: 'app-lichkiemtra',
  templateUrl: './lichkiemtra.component.html',
  styleUrls: ['./lichkiemtra.component.scss']
})
export class LichkiemtraComponent implements OnInit {
  data: BaiKiemTraDto[] = [];
  tenLop: string = '';
  
  thongBao: string = '';
  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private lopdangdayService: LopdangdayService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      const tenLopParam = params.get('tenLop');
      if (tenLopParam) {
        this.tenLop = tenLopParam;
        this.loadLichKiemTra(this.tenLop);
        
      }
    });
  }

  loadLichKiemTra(tenLop: string) {
    this.lopdangdayService.getLichKiemTraChoGiaoVien(tenLop).subscribe({
      next: res => {
         if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        this.data = res.data;
        this.cdr.detectChanges();
      },
      error: err => {
       
      }
    });
  }
}

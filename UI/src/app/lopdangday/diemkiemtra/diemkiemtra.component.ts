import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute,Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { KetQuaBaiKiemTra } from '../shared/lopdangday.model';
import { LopdangdayService } from '../shared/lopdangday.service';
@Component({
  selector: 'app-diemkiemtra',
  templateUrl: './diemkiemtra.component.html',
  styleUrls: ['./diemkiemtra.component.scss']
})
export class DiemkiemtraComponent implements OnInit {

  danhSachHocSinh: KetQuaBaiKiemTra[] = [];
  baiKiemTraId: string = '';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private lopdangdayService: LopdangdayService,
    private router: Router
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
          this.router.navigate(['/pages/error'])
          return;
        }
        this.danhSachHocSinh = res.data;
        this.cdr.detectChanges();
      },
      error: err => {
        
      }
    });
  }

  luuKetQua() {
    console.log('Danh sách cần lưu:', this.danhSachHocSinh);
    // TODO: Gọi API lưu nếu có
  }
}

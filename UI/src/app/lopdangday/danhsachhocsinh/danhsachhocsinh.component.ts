import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { LopdangdayService } from '../shared/lopdangday.service';
import { DanhSachHocSinh } from '../shared/lopdangday.model';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-danhsachhocsinh',
  templateUrl: './danhsachhocsinh.component.html',
  styleUrls: ['./danhsachhocsinh.component.scss']
})
export class DanhsachhocsinhComponent implements OnInit {
  danhSachHocSinh: DanhSachHocSinh [] = [];
  lopId: string;

  constructor(private lopDangDayService: LopdangdayService, private cdr: ChangeDetectorRef, private route: ActivatedRoute, ) { }

  ngOnInit(): void {
    this.lopId = this.route.parent?.snapshot.paramMap.get('id')!;
    this.loadDanhSachHocSinh();
  }

  loadDanhSachHocSinh() {   
    this.lopDangDayService.getDanhSachHocSinhLop(this.lopId)
      .subscribe(
        (response) => {
          this.danhSachHocSinh = response.data;
          this.cdr.detectChanges(); // Cập nhật view nếu cần thiết
          console.debug('Danh sách học sinh:', this.danhSachHocSinh);
        },
        (error) => {
          console.debug('Error fetching students:', error);
        }
      );
    }
}

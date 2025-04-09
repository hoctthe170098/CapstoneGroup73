import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { LopdangdayService } from '../shared/lopdangday.service';
import { DanhSachHocSinh } from '../shared/lopdangday.model';
import { ActivatedRoute, Router } from '@angular/router';
import { el } from 'date-fns/locale';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: "app-danhsachhocsinh",
  templateUrl: "./danhsachhocsinh.component.html",
  styleUrls: ["./danhsachhocsinh.component.scss"],
})
export class DanhsachhocsinhComponent implements OnInit {
  danhSachHocSinh: DanhSachHocSinh[] = [];
  tenLop: string;

  constructor(
    private lopDangDayService: LopdangdayService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.tenLop = this.route.parent?.snapshot.paramMap.get("tenLop");
    this.loadDanhSachHocSinh();
  }

  loadDanhSachHocSinh() {
    this.lopDangDayService.getDanhSachHocSinhLop(this.tenLop).subscribe(
      (response) => {
        if (!response.isError) {
          this.danhSachHocSinh = response.data;
          this.cdr.detectChanges();
          console.debug("Danh sách học sinh:", this.danhSachHocSinh);
        } else {
          if (response.message === 'Dữ liệu không tồn tại!'){ 
            this.toastr.error('Giáo viên id không hợp lê!', 'Lỗi');
          }
          else(response.code === 404)
            this.router.navigate(['/pages/error'])
        }
      },
      (error) => {
        console.debug("Error fetching students:", error);
      }
    );
  }
}

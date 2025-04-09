import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute,Router} from '@angular/router';
import { LopdangdayService } from '../shared/lopdangday.service';

@Component({
  selector: 'app-baocaodiemdanh',
  templateUrl: './baocaodiemdanh.component.html',
  styleUrls: ['./baocaodiemdanh.component.scss']
})
export class BaocaodiemdanhComponent implements OnInit {

  tenLop: string = '';
  danhSachNgay: string[] = [];
  danhSachHocSinh: {
    ten: string;
    code: string;
    trangThai: { [ngay: string]: string };
  }[] = [];

  constructor(
    private route: ActivatedRoute,
    private lopService: LopdangdayService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Lấy tên lớp từ param của route cha
    this.route.parent?.paramMap.subscribe(params => {
      const tenLopParam = params.get('tenLop');
      if (tenLopParam) {
        this.tenLop = tenLopParam;
        this.layBaoCaoDiemDanh();
        
      }
    });
  }

  layBaoCaoDiemDanh(): void {
    this.lopService.getBaoCaoDiemDanh(this.tenLop).subscribe({
      next: (res) => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }

        const data = res.data;
        if (!data || data.length === 0) {
          this.danhSachNgay = [];
          this.danhSachHocSinh = [];
          return;
        }

        // Danh sách các ngày đã điểm danh
        this.danhSachNgay = data.map((x: any) => x.ngay);

        // Gom học sinh theo code và trạng thái theo ngày
        const hocSinhMap: {
          [code: string]: { ten: string; trangThai: { [ngay: string]: string } };
        } = {};

        for (const baoCao of data) {
          for (const dd of baoCao.diemDanhs) {
            if (!hocSinhMap[dd.hocSinhCode]) {
              hocSinhMap[dd.hocSinhCode] = {
                ten: dd.tenHocSinh,
                trangThai: {}
              };
            }
            hocSinhMap[dd.hocSinhCode].trangThai[baoCao.ngay] = dd.trangThai;
          }
        }

        // Chuyển sang mảng để render trong bảng
        this.danhSachHocSinh = Object.entries(hocSinhMap).map(([code, info]) => ({
          code,
          ten: info.ten,
          trangThai: info.trangThai
        }));
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi lấy báo cáo điểm danh:', err);
        this.danhSachNgay = [];
        this.danhSachHocSinh = [];
      }
    });
  }
}

import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute,Router} from '@angular/router';
import { LopdangdayService } from '../shared/lopdangday.service';

@Component({
  selector: 'app-baocaodiem',
  templateUrl: './baocaodiem.component.html',
  styleUrls: ['./baocaodiem.component.scss']
})
export class BaocaodiemComponent implements OnInit {
  reportDates: string[] = [];
  students: any[] = [];
  tenLop: string = '';

  constructor(private lopService: LopdangdayService,  private router: Router,private route: ActivatedRoute,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe(params => {
      const tenLopParam = params.get('tenLop');
      if (tenLopParam) {
        this.tenLop = tenLopParam;
        this.fetchBaoCaoDiem();
        
      }
    });
  }

  fetchBaoCaoDiem(): void {
    this.lopService.getBaoCaoDiemHangNgay(this.tenLop).subscribe(res => {
      if (res.code === 404) {
        this.router.navigate(['/pages/error'])
        return;
      }
      if (res?.data && Array.isArray(res.data)) {
        const diemData = res.data;

        // lấy danh sách ngày
        this.reportDates = diemData.map((item: any) => this.formatNgay(item.ngay));

        // build student map
        const studentMap: { [key: string]: any } = {};

        diemData.forEach((entry: any) => {
          entry.diemDanhs.forEach((diem: any) => {
            if (!studentMap[diem.hocSinhCode]) {
              studentMap[diem.hocSinhCode] = {
                name: diem.tenHocSinh,
                scores: {}
              };
            }

            const dateKey = this.formatNgay(entry.ngay);
            studentMap[diem.hocSinhCode].scores[dateKey] = {
              homework: diem.diemBTVN ?? '-',
              classwork: diem.diemTrenLop ?? '-'
            };
          });
        });

        // convert map to array
        this.students = Object.values(studentMap);
        this.cdr.detectChanges();
      }
    });
  }

  formatNgay(ngay: string): string {
    const dateObj = new Date(ngay);
    return `${String(dateObj.getDate()).padStart(2, '0')}/${String(dateObj.getMonth() + 1).padStart(2, '0')}`;
  }
  getDiemClass(score: number): string {
    if (score == null || isNaN(score)) return 'diem-chua-co';
    return score > 6 ? 'diem-xanh' : 'diem-do';
  }
}

import { Component, OnInit,ChangeDetectorRef} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LopdangdayService } from '../shared/lopdangday.service';
@Component({
  selector: 'app-nhanxetdinhki',
  templateUrl: './nhanxetdinhki.component.html',
  styleUrls: ['./nhanxetdinhki.component.scss']
})
export class NhanxetdinhkiComponent implements OnInit {
  hocSinh: any;
  nhanXetMoi: string = '';
  dropdownIndex: number | null = null;
  lichSu: any[] = [];
  tenLop: string | null = null;
  constructor(private route: ActivatedRoute, private router: Router,private lopService: LopdangdayService,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const code = this.route.snapshot.paramMap.get('hocSinhId');
    this.tenLop = this.route.parent?.snapshot.paramMap.get('tenLop'); // 👈 Gán vào this
  
    if (code && this.tenLop) {
      this.loadNhanXetDinhKy(this.tenLop, code);
    }
  }
  loadNhanXetDinhKy(tenLop: string, code: string) {
    this.hocSinh = {
      code: code,
      ten: '' 
    };
  
    this.lopService.getNhanXetDinhKy(tenLop, code).subscribe({
      next: (res) => {
        const data = res.data;
  
        this.hocSinh = {
          code: data.hocSinhCode,
          ten: data.tenHocSinh
        };
  
        this.lichSu = data.danhSachNhanXet.map((item: any) => ({
          ngay: item.ngayNhanXet,
          noiDung: item.noiDungNhanXet
        }));
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Lỗi lấy nhận xét định kỳ:', err);
      }
    });
  }
  
  

  xacNhan() {
    if (!this.nhanXetMoi.trim()) return;
  
    const payload = {
      hocSinhCode: this.hocSinh.code,
      tenLop: this.tenLop,
      noiDungNhanXet: this.nhanXetMoi.trim()
    };
  
    this.lopService.createNhanXetDinhKy(payload).subscribe({
      next: (res) => {
        if (!res.isError) {
          // Reload danh sách nhận xét mới nhất
          this.loadNhanXetDinhKy(this.tenLop, this.hocSinh.code);
          this.nhanXetMoi = '';
          this.dropdownIndex = null;
        } else {
          alert(res.message); // tuỳ bạn xử lý
        }
      },
      error: (err) => {
        console.error('Lỗi tạo nhận xét:', err);
        alert(err?.error?.message || 'Tạo nhận xét thất bại!');
      }
    });
  }
  

  troVe() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  toggleDropdown(index: number) {
    this.dropdownIndex = this.dropdownIndex === index ? null : index;
  }

  suaNhanXet(item: any) {
    this.nhanXetMoi = item.noiDung;
    this.lichSu = this.lichSu.filter(x => x !== item);
    this.dropdownIndex = null;
  }

  xoaNhanXet(item: any) {
    this.lichSu = this.lichSu.filter(x => x !== item);
    this.dropdownIndex = null;
  }
}

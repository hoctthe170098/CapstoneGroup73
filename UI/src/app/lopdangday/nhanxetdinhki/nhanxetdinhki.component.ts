import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LopdangdayService } from '../shared/lopdangday.service';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
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
  idDangSua: string | null = null;
  denHanNhanXet: boolean = false;

  constructor(private route: ActivatedRoute, 
              private router: Router, 
              private lopService: LopdangdayService, 
              private cdr: ChangeDetectorRef, 
              private location: Location,
              private toastr: ToastrService
            ) { }

  ngOnInit(): void {
    const code = this.route.snapshot.paramMap.get('hocSinhId');
    this.tenLop = this.route.parent?.snapshot.paramMap.get('tenLop'); //  Gán vào this

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
        if(!res.isError){
          const data = res.data;
          this.hocSinh = {
            code: data.hocSinhCode,
            ten: data.tenHocSinh
          };
          this.lichSu = data.danhSachNhanXet.map((item: any) => ({
            id: item.id,
            ngay: item.ngayNhanXet,
            noiDung: item.noiDungNhanXet
          }));
          this.denHanNhanXet = data.denHanNhanXet;
          this.cdr.detectChanges();
        }else{
          if(res.code==404){
            this.router.navigate(['/pages/error'])
          }else this.toastr.error(res.message)
        }
      },
      error: (err) => {
        this.toastr.error('Lỗi lấy nhận xét định kỳ:', err);
      }
    });
  }



  xacNhan() {
    const noiDung = this.nhanXetMoi.trim();
    if (!noiDung) return;

    // Trường hợp chỉnh sửa
    if (this.idDangSua) {
      this.lopService.updateNhanXetDinhKy({
        id: this.idDangSua,
        noiDungNhanXet: noiDung
      }).subscribe({
        next: (res) => {
          if (!res.isError) {
            this.toastr.success(res.message);
            this.loadNhanXetDinhKy(this.tenLop, this.hocSinh.code); // reload danh sách
            this.nhanXetMoi = '';
            this.idDangSua = null;
          } else {
            alert(res.message);
          }
        },
        error: (err) => {
          alert(err?.error?.message || 'Lỗi cập nhật nhận xét!');
        }
      });

      return;
    }

    // Trường hợp TẠO MỚI nhận xét
    this.lopService.createNhanXetDinhKy({
      hocSinhCode: this.hocSinh.code,
      tenLop: this.tenLop!,
      noiDungNhanXet: noiDung
    }).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message);
          this.loadNhanXetDinhKy(this.tenLop, this.hocSinh.code);
          this.nhanXetMoi = '';
        } else {
          alert(res.message);
        }
      },
      error: (err) => {
        alert(err?.error?.message || 'Lỗi tạo nhận xét!');
      }
    });
  }



  troVe() {
    this.location.back(); 
  }

  toggleDropdown(index: number) {
    this.dropdownIndex = this.dropdownIndex === index ? null : index;
  }

  suaNhanXet(item: any) {
    this.nhanXetMoi = item.noiDung;
    this.idDangSua = item.id;
    this.dropdownIndex = null;
  }

  xoaNhanXet(item: any) {
    const xacNhan = confirm('Bạn có chắc chắn muốn xoá nhận xét này không?');
    if (!xacNhan) return;

    this.lopService.deleteNhanXetDinhKy(item.id).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message);
          this.loadNhanXetDinhKy(this.tenLop, this.hocSinh.code);
          this.dropdownIndex = null;
        } else {
          this.toastr.error(res.message);
        }
      },
      error: (err) => {
        alert(err?.error?.message || 'Xoá nhận xét thất bại!');
      }
    });
  }
}

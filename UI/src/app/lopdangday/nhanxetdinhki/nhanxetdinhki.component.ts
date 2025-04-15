import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-nhanxetdinhki',
  templateUrl: './nhanxetdinhki.component.html',
  styleUrls: ['./nhanxetdinhki.component.scss']
})
export class NhanxetdinhkiComponent implements OnInit {
  hocSinh: any;
  nhanXetMoi: string = '';
  dropdownIndex: number | null = null;

  lichSu = [
    {
      ngay: new Date('2021-02-01'),
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...'
    },
    {
      ngay: new Date('2021-02-01'),
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...'
    }
  ];

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    const code = this.route.snapshot.paramMap.get('hocSinhId');
    const ten = this.route.snapshot.queryParamMap.get('ten');
  
    this.hocSinh = {
      code: code,
      ten: ten
    };
  }
  

  xacNhan() {
    if (this.nhanXetMoi.trim()) {
      this.lichSu.unshift({
        ngay: new Date(),
        noiDung: this.nhanXetMoi.trim()
      });
      this.nhanXetMoi = '';
      this.dropdownIndex = null;
    }
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

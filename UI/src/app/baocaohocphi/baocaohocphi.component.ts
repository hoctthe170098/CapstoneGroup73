import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baocaohocphi',
  templateUrl: './baocaohocphi.component.html',
  styleUrls: ['./baocaohocphi.component.scss']
})
export class BaocaohocphiComponent implements OnInit {

  baoCaoHocPhi = Array.from({ length: 9 }).map(() => ({
    hocSinh: 'Bùi Ngọc Dũng',
    soBuoi: 10,
    hocPhi: '500.000 VND',
    trangThai: 'Đã nộp'
  }));

  constructor() { }

  ngOnInit(): void { }

}

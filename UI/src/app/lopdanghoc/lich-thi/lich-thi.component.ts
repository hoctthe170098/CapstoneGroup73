import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-lich-thi',
  templateUrl: './lich-thi.component.html',
  styleUrls: ['./lich-thi.component.scss']
})
export class LichThiComponent implements OnInit {

  lichThi: any[] = [];

  constructor() { }

  ngOnInit(): void {
    this.lichThi = [
      {
        ten: 'Kiểm tra cuối kì 2 toán 10',
        ngay: '18/03/2021',
        trangThai: 'Chưa bắt đầu',
        diem: 'Chưa có điểm',
        nhanXet: 'Chưa có nhận xét'
      },
        
    ];
  }

}
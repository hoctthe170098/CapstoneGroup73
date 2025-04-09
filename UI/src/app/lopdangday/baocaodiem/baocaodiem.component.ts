import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baocaodiem',
  templateUrl: './baocaodiem.component.html',
  styleUrls: ['./baocaodiem.component.scss']
})
export class BaocaodiemComponent implements OnInit {

  reportDates: string[] = [
    '09/4', '11/4', '16/4', '18/4', '23/4',
    '25/4', '30/4', '02/5', '04/5'
  ];

  students = [
    {
      name: 'Bùi Ngọc Dũng',
      scores: {
        '09/4': { homework: 8, classwork: 9 },
        '11/4': { homework: 8, classwork: 9 },
        '16/4': { homework: 8, classwork: 9 },
        '18/4': { homework: 8, classwork: 9 },
        '23/4': { homework: 8, classwork: 9 },
        '25/4': { homework: 8, classwork: 9 },
        '30/4': { homework: 8, classwork: 9 },
        '02/5': { homework: 8, classwork: 9 },
        '04/5': { homework: 8, classwork: 9 }
      }
    },
    {
      name: 'Nguyễn Văn A',
      scores: {
        '09/4': { homework: 9, classwork: 9 },
        '11/4': { homework: 7, classwork: 8 },
        '16/4': { homework: 10, classwork: 10 },
        '18/4': { homework: 9, classwork: 10 },
        '23/4': { homework: 9, classwork: 9 },
        '25/4': { homework: 8, classwork: 9 },
        '30/4': { homework: 7, classwork: 8 },
        '02/5': { homework: 9, classwork: 9 },
        '04/5': { homework: 8, classwork: 10 }
      }
    },
    {
      name: 'Trần Thị B',
      scores: {
        '09/4': { homework: 10, classwork: 9 },
        '11/4': { homework: 9, classwork: 9 },
        '16/4': { homework: 9, classwork: 8 },
        '18/4': { homework: 9, classwork: 9 },
        '23/4': { homework: 10, classwork: 10 },
        '25/4': { homework: 9, classwork: 9 },
        '30/4': { homework: 8, classwork: 7 },
        '02/5': { homework: 9, classwork: 9 },
        '04/5': { homework: 10, classwork: 10 }
      }
    },
    {
      name: 'Phạm Văn C',
      scores: {
        '09/4': { homework: 7, classwork: 8 },
        '11/4': { homework: 6, classwork: 7 },
        '16/4': { homework: 8, classwork: 8 },
        '18/4': { homework: 7, classwork: 7 },
        '23/4': { homework: 8, classwork: 8 },
        '25/4': { homework: 8, classwork: 8 },
        '30/4': { homework: 9, classwork: 9 },
        '02/5': { homework: 9, classwork: 8 },
        '04/5': { homework: 9, classwork: 9 }
      }
    },
    {
      name: 'Lê Thị D',
      scores: {
        '09/4': { homework: 9, classwork: 10 },
        '11/4': { homework: 10, classwork: 10 },
        '16/4': { homework: 9, classwork: 9 },
        '18/4': { homework: 9, classwork: 9 },
        '23/4': { homework: 8, classwork: 8 },
        '25/4': { homework: 10, classwork: 10 },
        '30/4': { homework: 9, classwork: 9 },
        '02/5': { homework: 10, classwork: 10 },
        '04/5': { homework: 9, classwork: 10 }
      }
    },
    {
      name: 'Đỗ Mạnh E',
      scores: {
        '09/4': { homework: 6, classwork: 7 },
        '11/4': { homework: 7, classwork: 8 },
        '16/4': { homework: 8, classwork: 7 },
        '18/4': { homework: 6, classwork: 7 },
        '23/4': { homework: 7, classwork: 7 },
        '25/4': { homework: 8, classwork: 8 },
        '30/4': { homework: 7, classwork: 6 },
        '02/5': { homework: 8, classwork: 8 },
        '04/5': { homework: 7, classwork: 7 }
      }
    },
    {
      name: 'Ngô Quỳnh F',
      scores: {
        '09/4': { homework: 10, classwork: 10 },
        '11/4': { homework: 9, classwork: 10 },
        '16/4': { homework: 10, classwork: 10 },
        '18/4': { homework: 9, classwork: 10 },
        '23/4': { homework: 10, classwork: 10 },
        '25/4': { homework: 10, classwork: 10 },
        '30/4': { homework: 9, classwork: 9 },
        '02/5': { homework: 9, classwork: 9 },
        '04/5': { homework: 10, classwork: 10 }
      }
    },
    {
      name: 'Vũ Đức G',
      scores: {
        '09/4': { homework: 8, classwork: 8 },
        '11/4': { homework: 8, classwork: 9 },
        '16/4': { homework: 8, classwork: 8 },
        '18/4': { homework: 7, classwork: 7 },
        '23/4': { homework: 7, classwork: 7 },
        '25/4': { homework: 8, classwork: 8 },
        '30/4': { homework: 9, classwork: 9 },
        '02/5': { homework: 9, classwork: 8 },
        '04/5': { homework: 9, classwork: 9 }
      }
    },
    {
      name: 'Lý Minh H',
      scores: {
        '09/4': { homework: 6, classwork: 7 },
        '11/4': { homework: 7, classwork: 8 },
        '16/4': { homework: 6, classwork: 7 },
        '18/4': { homework: 6, classwork: 6 },
        '23/4': { homework: 7, classwork: 6 },
        '25/4': { homework: 8, classwork: 7 },
        '30/4': { homework: 9, classwork: 9 },
        '02/5': { homework: 8, classwork: 8 },
        '04/5': { homework: 8, classwork: 9 }
      }
    },
    {
      name: 'Hoàng Văn I',
      scores: {
        '09/4': { homework: 9, classwork: 10 },
        '11/4': { homework: 9, classwork: 10 },
        '16/4': { homework: 9, classwork: 9 },
        '18/4': { homework: 9, classwork: 10 },
        '23/4': { homework: 10, classwork: 10 },
        '25/4': { homework: 9, classwork: 9 },
        '30/4': { homework: 10, classwork: 10 },
        '02/5': { homework: 10, classwork: 9 },
        '04/5': { homework: 10, classwork: 10 }
      }
    }
  ];

  constructor() {}

  ngOnInit(): void {}
}

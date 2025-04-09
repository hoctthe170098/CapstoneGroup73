import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baocaodiemdanh',
  templateUrl: './baocaodiemdanh.component.html',
  styleUrls: ['./baocaodiemdanh.component.scss']
})
export class BaocaodiemdanhComponent implements OnInit {

  attendanceDates: string[] = [
    '09/4', '11/4', '16/4', '18/4', '23/4', '25/4', '30/4',
    '02/5', '04/5', '09/5', '11/5', '13/5', '18/5', '20/5',
    '25/5', '27/5', '30/5', '04/6', '06/6'
  ];

  students = [
    {
      name: 'Bùi Ngọc Dũng',
      attendance: {
        '09/4': true, '11/4': true, '16/4': null, '18/4': true, '23/4': true,
        '25/4': true, '30/4': true, '02/5': true, '04/5': true, '09/5': true,
        '11/5': true, '13/5': false, '18/5': true, '20/5': true, '25/5': true,
        '27/5': true, '30/5': false, '04/6': true, '06/6': true
      }
    },
    {
      name: 'Nguyễn Văn A',
      attendance: {
        '09/4': true, '11/4': true, '16/4': null, '18/4': true, '23/4': false,
        '25/4': true, '30/4': true, '02/5': false, '04/5': true, '09/5': true,
        '11/5': true, '13/5': true, '18/5': true, '20/5': false, '25/5': true,
        '27/5': true, '30/5': true, '04/6': true, '06/6': true
      }
    },
    {
      name: 'Trần Thị B',
      attendance: {
        '09/4': true, '11/4': true, '16/4': null, '18/4': true, '23/4': true,
        '25/4': false, '30/4': true, '02/5': true, '04/5': false, '09/5': true,
        '11/5': false, '13/5': true, '18/5': true, '20/5': true, '25/5': true,
        '27/5': true, '30/5': true, '04/6': false, '06/6': true
      }
    },
    {
      name: 'Lê Văn C',
      attendance: {
        '09/4': false, '11/4': true, '16/4': null, '18/4': true, '23/4': true,
        '25/4': true, '30/4': true, '02/5': true, '04/5': true, '09/5': false,
        '11/5': true, '13/5': true, '18/5': true, '20/5': true, '25/5': false,
        '27/5': true, '30/5': true, '04/6': true, '06/6': false
      }
    },
    {
      name: 'Phạm Thị D',
      attendance: {
        '09/4': true, '11/4': false, '16/4': null, '18/4': true, '23/4': true,
        '25/4': true, '30/4': false, '02/5': true, '04/5': true, '09/5': true,
        '11/5': true, '13/5': false, '18/5': true, '20/5': true, '25/5': true,
        '27/5': false, '30/5': true, '04/6': true, '06/6': true
      }
    },
    {
      name: 'Đỗ Mạnh E',
      attendance: {
        '09/4': true, '11/4': true, '16/4': null, '18/4': true, '23/4': true,
        '25/4': true, '30/4': true, '02/5': true, '04/5': true, '09/5': true,
        '11/5': false, '13/5': false, '18/5': true, '20/5': true, '25/5': true,
        '27/5': true, '30/5': true, '04/6': true, '06/6': false
      }
    },
    {
      name: 'Ngô Quỳnh F',
      attendance: {
        '09/4': false, '11/4': true, '16/4': null, '18/4': true, '23/4': false,
        '25/4': true, '30/4': true, '02/5': false, '04/5': true, '09/5': true,
        '11/5': true, '13/5': true, '18/5': false, '20/5': true, '25/5': true,
        '27/5': true, '30/5': false, '04/6': true, '06/6': true
      }
    },
    {
      name: 'Vũ Đức G',
      attendance: {
        '09/4': true, '11/4': true, '16/4': null, '18/4': true, '23/4': true,
        '25/4': true, '30/4': true, '02/5': true, '04/5': true, '09/5': true,
        '11/5': true, '13/5': true, '18/5': true, '20/5': true, '25/5': true,
        '27/5': true, '30/5': true, '04/6': true, '06/6': true
      }
    },
    {
      name: 'Lý Minh H',
      attendance: {
        '09/4': true, '11/4': false, '16/4': null, '18/4': true, '23/4': true,
        '25/4': false, '30/4': true, '02/5': true, '04/5': true, '09/5': false,
        '11/5': true, '13/5': true, '18/5': true, '20/5': false, '25/5': true,
        '27/5': true, '30/5': true, '04/6': true, '06/6': true
      }
    },
    {
      name: 'Hoàng Văn I',
      attendance: {
        '09/4': true, '11/4': true, '16/4': null, '18/4': true, '23/4': true,
        '25/4': true, '30/4': true, '02/5': true, '04/5': true, '09/5': true,
        '11/5': true, '13/5': true, '18/5': true, '20/5': true, '25/5': true,
        '27/5': true, '30/5': true, '04/6': true, '06/6': true
      }
    }
  ];
  

  constructor() {}


  ngOnInit(): void {
  }

}

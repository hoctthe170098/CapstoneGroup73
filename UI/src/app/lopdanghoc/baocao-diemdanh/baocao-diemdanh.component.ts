import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baocao-diemdanh',
  templateUrl: './baocao-diemdanh.component.html',
  styleUrls: ['./baocao-diemdanh.component.scss']
})
export class BaocaoDiemdanhComponent implements OnInit {

  baoCaoDiemDanh = [
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Vắng' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Vắng' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' },
    { ngay: '18/03/2021', thoiGian: '17h - 19h', phong: 'DE-191', giaoVien: 'Bùi Ngọc Dũng', trangThai: 'Có mặt' }
  ];

  constructor() { }

  ngOnInit(): void {
  }

}

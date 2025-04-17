import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-baocao-diem',
  templateUrl: './baocao-diem.component.html',
  styleUrls: ['./baocao-diem.component.scss']
})
export class BaocaoDiemComponent implements OnInit {

  diemTrungBinh = {
    diemTrenLop: 10,
    diemBaiTap: 9,
    diemKiemTra: 8.5
  };

  nhanXetDinhKy = [
    { ngay: '01/02/2021', nhanXet: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...' },
    { ngay: '07/02/2021', nhanXet: 'Có tiến bộ rõ rệt so với tuần trước.' },
    { ngay: '14/02/2021', nhanXet: 'Cần chú ý hơn đến bài tập về nhà.' },
    { ngay: '21/02/2021', nhanXet: 'Chăm chỉ và nghiêm túc trong học tập.' },
    { ngay: '28/02/2021', nhanXet: 'Chưa hoàn thành bài tập đúng hạn.' },
    { ngay: '07/03/2021', nhanXet: 'Thái độ học tập tốt, cần giữ vững.' },
    { ngay: '14/03/2021', nhanXet: 'Có dấu hiệu sao nhãng, cần cải thiện.' },
    { ngay: '21/03/2021', nhanXet: 'Rất tích cực phát biểu trong giờ học.' },
    { ngay: '28/03/2021', nhanXet: 'Thường xuyên quên làm bài tập.' },
    { ngay: '04/04/2021', nhanXet: 'Tiếp thu bài nhanh, làm bài đầy đủ.' },
    { ngay: '11/04/2021', nhanXet: 'Tăng cường luyện tập ở nhà để nâng cao kỹ năng.' },
    { ngay: '18/04/2021', nhanXet: 'Đã tiến bộ hơn so với các tuần trước.' },
    { ngay: '25/04/2021', nhanXet: 'Cần tập trung hơn khi làm bài kiểm tra.' }
  ];

  diemHangNgay = Array.from({ length: 15 }, (_, i) => ({
    ngay: `18/03/2021`,
    diemTrenLop: '10/10',
    diemBTVN: '9/10',
    nhanXet: `Trên lớp: Cố gắng tiếp tục phát huy (${i + 1}) | BTVN: Hoàn thành ổn, cần chi tiết hơn (${i + 1})`
  }));
  

  diemKiemTra = Array.from({ length: 12 }, (_, i) => ({
    ten: `Kiểm tra số ${i + 1}`,
    ngay: `20/03/2021`,
    trangThai: i % 2 === 0 ? 'Đã kiểm tra' : 'Chưa bắt đầu',
    diem: i % 2 === 0 ? '8/10' : 'Chưa có điểm',
    nhanXet: i % 2 === 0 ? 'Bài làm tốt, trình bày cần rõ ràng' : 'Chưa có nhận xét'
  }));

  constructor() {}

  ngOnInit(): void {}
}

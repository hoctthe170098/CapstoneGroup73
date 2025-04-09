import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-chitiet-baitap',
  templateUrl: './chitiet-baitap.component.html',
  styleUrls: ['./chitiet-baitap.component.scss']
})
export class ChitietBaitapComponent implements OnInit {

  tenLop: string = '';
  baiTapId: string = '';
  isEditModalOpen = false;
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
      this.baiTapId = params.get('baiTapId') || '';
      console.log('Tên lớp:', this.tenLop);
      console.log('ID bài tập:', this.baiTapId);
      
      
    });
  }

  constructor(private route: ActivatedRoute) {}
  answers = [
    {
      tenHocSinh: 'Bùi Ngọc Dũng',
      thoiGianNop: '18:30',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB',
       
      }
    },
    {
      tenHocSinh: 'Ngô Minh Kiên',
      thoiGianNop: '18:30',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB',
        
      }
    },
    {
      tenHocSinh: 'Nguyễn Tuấn Anh',
      thoiGianNop: '18:30',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB',
        
      }
    }
  ];
  editBaiTap = {
    tieuDe: 'Ôn Tập Toán Cấp 3',
    noiDung: `Lorem ipsum dolor sit amet, consectetur adipiscing elit...`,
    gio: '10:30',
    trangThai: 'Chưa mở',
    file: {
      name: 'File Title.pdf',
      size: '313 KB',
      
    }
  };
  
  openEditModal() {
    this.isEditModalOpen = true;
  }
  
  closeEditModal() {
    this.isEditModalOpen = false;
  }
  
  onEditFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.editBaiTap.file = {
        name: file.name,
        size: `${Math.round(file.size / 1024)} KB`,
        
      };
    }
  }
  
  removeEditFile(): void {
    this.editBaiTap.file = null;
  }
  
  confirmEdit(): void {
    console.log('📤 Dữ liệu chỉnh sửa:', this.editBaiTap);
    this.closeEditModal();
  }
}

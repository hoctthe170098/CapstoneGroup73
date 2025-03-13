import { Component } from '@angular/core';

@Component({
  selector: 'app-import-giaovien',
  templateUrl: './import-giaovien.component.html',
  styleUrls: ['./import-giaovien.component.scss']
})
export class ImportGiaovienComponent {
  // Biến để kiểm soát trạng thái chọn file
  fileSelected: boolean = false;
  fileName: string = '';

  importedTeachers: any[] = [];

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.fileName = file.name;
      this.fileSelected = true;
      this.mockParseExcel();
    }
  }

  mockParseExcel() {
    this.importedTeachers = [
      {
        code: 'IM171450',
        ten: 'Trần Văn A',
        gioiTinh: 'Nam',
        ngaySinh: new Date(2003, 6, 15), 
        email: 'tranva@example.com',
        soDienThoai: '0123-456-789',
        status: 'Hoạt động',
        diaChi: '123 Đường ABC, Quận 1, TP HCM',
        truongDangDay: 'Trường ABC',
        coso: 'Hoàng Văn Thái',  
        lopHocs: ['Toán', 'Lý'],
        showDetails: false
      },
      {
        code: 'IM171466',
        ten: 'Lê Thị B',
        gioiTinh: 'Nữ',
        ngaySinh: new Date(2003, 3, 20),
        email: 'lethib@example.com',
        soDienThoai: '0987-654-321',
        status: 'Hoạt động',
        diaChi: '456 Đường XYZ, Quận 2, TP HCM',
        truongDangDay: 'Trường DEF',
        coso: 'Hoàng Văn Thái',  
        lopHocs: ['Hóa', 'Sinh'],
        showDetails: false
      },
      {
        code: 'IM171499',
        ten: 'Phạm Văn C',
        gioiTinh: 'Nam',
        ngaySinh: new Date(2003, 1, 10),
        email: 'phamvc@example.com',
        soDienThoai: '0905-123-456',
        status: 'Hoạt động',
        diaChi: '789 Đường QWE, Quận 3, TP HCM',
        truongDangDay: 'Trường GHI',
        coso: 'Hoàng Văn Thái',  
        lopHocs: ['Anh', 'Văn'],
        showDetails: false
      }
    ];
  }

  toggleDetails(index: number) {
    this.importedTeachers[index].showDetails = !this.importedTeachers[index].showDetails;
  }

  onCancel() {
    this.fileSelected = false;
    this.fileName = '';
    this.importedTeachers = [];
  }

  onConfirmImport() {
    alert('Đã xác nhận import file: ' + this.fileName);
  }

  onDropFile(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0];
      this.fileName = file.name;
      this.fileSelected = true;
      this.mockParseExcel();
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  goBack() {
    window.history.back();
  }
}

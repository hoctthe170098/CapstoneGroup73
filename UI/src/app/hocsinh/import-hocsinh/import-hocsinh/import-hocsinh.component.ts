import { Component } from '@angular/core';

@Component({
  selector: 'app-import-hocsinh',
  templateUrl: './import-hocsinh.component.html',
  styleUrls: ['./import-hocsinh.component.scss']
})
export class ImportHocsinhComponent {
  // Biến để kiểm soát trạng thái chọn file
  fileSelected: boolean = false;
  fileName: string = '';

  // Mảng chứa danh sách học viên được import (dữ liệu mẫu)
  importedStudents: any[] = [];

  // Khi người dùng chọn file (thông qua input file)
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.fileName = file.name;
      this.fileSelected = true;
      this.mockParseExcel();
    }
  }

  // Giả lập việc parse file Excel và tạo dữ liệu mẫu
  mockParseExcel() {
    this.importedStudents = [
      {
        code: 'IM171450',
        ten: 'Trần Văn A',
        gioiTinh: 'Nam',
        ngaySinh: new Date(2003, 6, 15), // 6 = July
        email: 'tranva@example.com',
        soDienThoai: '0123-456-789',
        status: 'Hoạt động',
        diaChi: '123 Đường ABC, Quận 1, TP HCM',
        truongDangHoc: 'Trường ABC',
        chinhSach: 'Cơ bản',
        lopHocs: ['Toán', 'Lý'],
        showDetails: false
      },
      {
        code: 'IM171466',
        ten: 'Lê Thị B',
        gioiTinh: 'Nữ',
        ngaySinh: new Date(2003, 3, 20), // 3 = April (hiển thị May)
        email: 'lethib@example.com',
        soDienThoai: '0987-654-321',
        status: 'Hoạt động',
        diaChi: '456 Đường XYZ, Quận 2, TP HCM',
        truongDangHoc: 'Trường DEF',
        chinhSach: 'Nâng cao',
        lopHocs: ['Hóa', 'Sinh'],
        showDetails: false
      },
      {
        code: 'IM171499',
        ten: 'Phạm Văn C',
        gioiTinh: 'Nam',
        ngaySinh: new Date(2003, 1, 10), // 1 = February (hiển thị March)
        email: 'phamvc@example.com',
        soDienThoai: '0905-123-456',
        status: 'Hoạt động',
        diaChi: '789 Đường QWE, Quận 3, TP HCM',
        truongDangHoc: 'Trường GHI',
        chinhSach: 'Ưu đãi',
        lopHocs: ['Anh', 'Văn'],
        showDetails: false
      }
    ];
  }

  // Toggle hiển thị chi tiết cho một học viên trong bảng
  toggleDetails(index: number) {
    this.importedStudents[index].showDetails = !this.importedStudents[index].showDetails;
  }

  // Các nút hành động ở phần file preview
  onCancel() {
    // Reset trạng thái chọn file và danh sách học viên import
    this.fileSelected = false;
    this.fileName = '';
    this.importedStudents = [];
  }

  onConfirmImport() {
    alert('Đã xác nhận import file: ' + this.fileName);
    // Ở đây bạn có thể gọi API xử lý import nếu có
  }

  // Hỗ trợ drag & drop file (nếu cần)
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
